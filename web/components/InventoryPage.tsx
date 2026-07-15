"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { apiFetch, queryPath } from "../lib/api";
import type { InventoryTransaction, Part } from "../lib/types";
import { EmptyState, ErrorNotice, Modal, PageHeader, Spinner, StatusPill } from "./Ui";

type TxAction = "receive" | "issue" | "return";

export function InventoryPage() {
  const [parts, setParts] = useState<Part[]>([]);
  const [allParts, setAllParts] = useState<Part[]>([]);
  const [transactions, setTransactions] = useState<InventoryTransaction[]>([]);
  const [partIdFilter, setPartIdFilter] = useState("");
  const [skip, setSkip] = useState(0);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");
  const [modal, setModal] = useState<"create" | "edit" | TxAction | null>(null);
  const [selected, setSelected] = useState<Part | null>(null);
  const [form, setForm] = useState({ partNumber: "", name: "", description: "", unitOfMeasure: "pcs", isActive: true });
  const [quantity, setQuantity] = useState(1);
  const [notes, setNotes] = useState("");

  const loadParts = useCallback(async () => {
    setLoading(true);
    try {
      const [partData, allPartData] = await Promise.all([
        apiFetch<Part[]>(queryPath("/v1/parts", { skip, take: 12 })),
        apiFetch<Part[]>(queryPath("/v1/parts", { skip: 0, take: 1000 })),
      ]);
      setParts(partData);
      setAllParts(allPartData);
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Parçalar yüklenemedi.");
    } finally {
      setLoading(false);
    }
  }, [skip]);

  const loadTransactions = useCallback(async () => {
    try {
      setTransactions(await apiFetch<InventoryTransaction[]>(queryPath("/v1/inventory/transactions", { skip: 0, take: 30, partId: partIdFilter || undefined })));
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Stok hareketleri yüklenemedi.");
    }
  }, [partIdFilter]);

  useEffect(() => {
    let active = true;
    void (async () => {
      setLoading(true);
      try {
        const [partData, allPartData] = await Promise.all([
          apiFetch<Part[]>(queryPath("/v1/parts", { skip, take: 12 })),
          apiFetch<Part[]>(queryPath("/v1/parts", { skip: 0, take: 1000 })),
        ]);
        if (active) {
          setParts(partData);
          setAllParts(allPartData);
        }
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Parçalar yüklenemedi.");
      } finally {
        if (active) setLoading(false);
      }
    })();
    return () => { active = false; };
  }, [skip]);
  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const tx = await apiFetch<InventoryTransaction[]>(queryPath("/v1/inventory/transactions", { skip: 0, take: 30, partId: partIdFilter || undefined }));
        if (active) setTransactions(tx);
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Stok hareketleri yüklenemedi.");
      }
    })();
    return () => { active = false; };
  }, [partIdFilter]);

  function openCreate() {
    setForm({ partNumber: "", name: "", description: "", unitOfMeasure: "pcs", isActive: true });
    setModal("create");
  }

  function openEdit(part: Part) {
    setSelected(part);
    setForm({ partNumber: part.partNumber, name: part.name, description: part.description || "", unitOfMeasure: part.unitOfMeasure, isActive: part.isActive });
    setModal("edit");
  }

  async function savePart(event: React.FormEvent) {
    event.preventDefault();
    setSaving(true);
    try {
      if (modal === "create") await apiFetch("/v1/parts", { method: "POST", body: JSON.stringify(form) });
      else if (selected) await apiFetch(`/v1/parts/${selected.id}`, { method: "PATCH", body: JSON.stringify({ id: selected.id, ...form }) });
      setNotice(modal === "create" ? "Parça oluşturuldu." : "Parça güncellendi.");
      setModal(null);
      await loadParts();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Parça kaydedilemedi.");
    } finally {
      setSaving(false);
    }
  }

  async function submitTransaction(event: React.FormEvent) {
    event.preventDefault();
    if (!selected || !modal || !["receive", "issue", "return"].includes(modal)) return;
    setSaving(true);
    try {
      await apiFetch(`/v1/parts/${selected.id}/${modal}`, { method: "POST", body: JSON.stringify({ partId: selected.id, quantity, notes: notes || null }) });
      setNotice("Stok hareketi kaydedildi.");
      setModal(null);
      setQuantity(1);
      setNotes("");
      await loadParts();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Stok hareketi kaydedilemedi.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <>
      <PageHeader eyebrow="Stok ve parça yönetimi" title="Envanter" description="Parça kartlarını yönetin, hareketleri kayıt altına alın ve stok seviyelerini takip edin." action={<button className="primary-button" onClick={openCreate}>+ Yeni parça</button>} />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <section className="card overflow-hidden">
        <div className="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 px-5 py-4"><div><h2 className="font-black text-ink">Parça kartları</h2><p className="mt-1 text-xs text-slate-400">Receive, issue ve return işlemleri işlem günlüğüne yazılır.</p></div><span className="text-xs text-slate-400">{parts.length} parça</span></div>
        {loading ? <div className="flex justify-center py-16 text-teal"><Spinner /></div> : parts.length === 0 ? <EmptyState label="Henüz parça bulunmuyor." /> : <div className="overflow-x-auto"><table className="w-full text-left text-sm"><thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500"><tr><th className="px-5 py-3">Parça</th><th className="px-5 py-3">Numara</th><th className="px-5 py-3">Birim</th><th className="px-5 py-3">Durum</th><th className="px-5 py-3" /></tr></thead><tbody className="divide-y divide-slate-100">{parts.map((part) => <tr key={part.id} className="hover:bg-slate-50"><td className="px-5 py-4"><Link href={`/inventory/${part.id}`} className="font-bold text-ink hover:text-teal">{part.name}</Link><p className="mt-1 text-xs text-slate-400">{part.description || "Açıklama yok"}</p></td><td className="px-5 py-4 text-slate-600">{part.partNumber}</td><td className="px-5 py-4 text-slate-600">{part.unitOfMeasure}</td><td className="px-5 py-4"><StatusPill value={part.isActive} /></td><td className="whitespace-nowrap px-5 py-4 text-right"><button className="mr-3 text-xs font-bold text-teal" onClick={() => openEdit(part)}>Düzenle</button><button className="mr-3 text-xs font-bold text-emerald-700" onClick={() => { setSelected(part); setQuantity(1); setNotes(""); setModal("receive"); }}>Al</button><button className="mr-3 text-xs font-bold text-rose-700" onClick={() => { setSelected(part); setQuantity(1); setNotes(""); setModal("issue"); }}>Çıkış</button><button className="text-xs font-bold text-amber-700" onClick={() => { setSelected(part); setQuantity(1); setNotes(""); setModal("return"); }}>İade</button></td></tr>)}</tbody></table></div>}
        <div className="flex items-center justify-between border-t border-slate-100 px-5 py-4"><button className="secondary-button text-xs" disabled={skip === 0} onClick={() => setSkip(Math.max(0, skip - 12))}>← Önceki</button><span className="text-xs text-slate-400">Sayfa {skip / 12 + 1}</span><button className="secondary-button text-xs" disabled={parts.length < 12} onClick={() => setSkip(skip + 12)}>Sonraki →</button></div>
      </section>
      <section className="card mt-6 overflow-hidden"><div className="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 px-5 py-4"><div><h2 className="font-black text-ink">Stok hareketleri</h2><p className="mt-1 text-xs text-slate-400">İsterseniz belirli bir parça ile filtreleyin.</p></div><select className="field max-w-xs" value={partIdFilter} onChange={(event) => { setPartIdFilter(event.target.value); setSkip(0); }}><option value="">Tüm parçalar</option>{allParts.map((part) => <option key={part.id} value={part.id}>{part.partNumber} · {part.name}</option>)}</select></div>{transactions.length === 0 ? <EmptyState label="Henüz stok hareketi bulunmuyor." /> : <div className="divide-y divide-slate-100">{transactions.map((tx) => <div key={tx.id} className="flex flex-wrap items-center justify-between gap-4 px-5 py-4"><div><p className="text-sm font-bold text-ink">{tx.partName} <span className="ml-2 text-xs font-normal text-slate-400">{tx.partNumber}</span></p><p className="mt-1 text-xs text-slate-400">{new Date(tx.transactionDate).toLocaleString("tr-TR")} · {tx.notes || "Not yok"}</p></div><div className="flex items-center gap-4"><StatusPill value={tx.type} /><span className="font-black text-ink">{tx.quantity} {tx.unitOfMeasure}</span></div></div>)}</div>}</section>
      {(modal === "create" || modal === "edit") && <Modal title={modal === "create" ? "Yeni parça" : "Parçayı düzenle"} onClose={() => setModal(null)}><form className="space-y-5" onSubmit={savePart}><div className="grid gap-4 sm:grid-cols-2"><label className="text-sm font-semibold text-ink">Parça numarası<input className="field mt-2" required value={form.partNumber} disabled={modal === "edit"} onChange={(event) => setForm({ ...form, partNumber: event.target.value })} /></label><label className="text-sm font-semibold text-ink">Parça adı<input className="field mt-2" required value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} /></label><label className="text-sm font-semibold text-ink">Birim<input className="field mt-2" required value={form.unitOfMeasure} onChange={(event) => setForm({ ...form, unitOfMeasure: event.target.value })} /></label><label className="flex items-center gap-2 self-end pb-3 text-sm text-slate-600"><input type="checkbox" checked={form.isActive} onChange={(event) => setForm({ ...form, isActive: event.target.checked })} /> Aktif</label></div><label className="block text-sm font-semibold text-ink">Açıklama<textarea className="field mt-2 min-h-24" value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} /></label><div className="flex justify-end gap-3"><button type="button" className="secondary-button" onClick={() => setModal(null)}>Vazgeç</button><button className="primary-button" disabled={saving}>{saving && <Spinner />} Kaydet</button></div></form></Modal>}
      {modal && ["receive", "issue", "return"].includes(modal) && selected && <Modal title={`${selected.name} · ${modal === "receive" ? "Stok alışı" : modal === "issue" ? "Stok çıkışı" : "Stok iadesi"}`} onClose={() => { setModal(null); setQuantity(1); setNotes(""); }}><form className="space-y-5" onSubmit={submitTransaction}><label className="block text-sm font-semibold text-ink">Miktar<input className="field mt-2" type="number" min="1" required value={quantity} onChange={(event) => setQuantity(Number(event.target.value))} /></label><label className="block text-sm font-semibold text-ink">Not<textarea className="field mt-2 min-h-24" value={notes} onChange={(event) => setNotes(event.target.value)} placeholder="İş emri, tedarikçi veya iade açıklaması" /></label><div className="flex justify-end gap-3"><button type="button" className="secondary-button" onClick={() => { setModal(null); setQuantity(1); setNotes(""); }}>Vazgeç</button><button className="primary-button" disabled={saving}>{saving && <Spinner />} Hareketi kaydet</button></div></form></Modal>}
    </>
  );
}
