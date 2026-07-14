"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { apiFetch, queryPath } from "../lib/api";
import type { Asset, WorkOrder } from "../lib/types";
import { EmptyState, ErrorNotice, Modal, PageHeader, Spinner, StatusPill } from "./Ui";

const statuses = ["Requested", "Approved", "Rejected", "Assigned", "InProgress", "OnHold", "Completed", "Verified", "Closed"];

export function WorkOrdersPage() {
  const [items, setItems] = useState<WorkOrder[]>([]);
  const [assets, setAssets] = useState<Asset[]>([]);
  const [skip, setSkip] = useState(0);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");
  const [modal, setModal] = useState<"create" | "status" | null>(null);
  const [selected, setSelected] = useState<WorkOrder | null>(null);
  const [assetId, setAssetId] = useState("");
  const [description, setDescription] = useState("");
  const [newStatus, setNewStatus] = useState("Requested");
  const [permitApproved, setPermitApproved] = useState(false);
  const [eSignatureCaptured, setESignatureCaptured] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      setItems(await apiFetch<WorkOrder[]>(queryPath("/v1/work-orders", { skip, take: 12 })));
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "İş emirleri yüklenemedi.");
    } finally {
      setLoading(false);
    }
  }, [skip]);

  useEffect(() => {
    let active = true;
    void (async () => {
      setLoading(true);
      try {
        const orders = await apiFetch<WorkOrder[]>(queryPath("/v1/work-orders", { skip, take: 12 }));
        if (active) setItems(orders);
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "İş emirleri yüklenemedi.");
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
        const assetData = await apiFetch<Asset[]>(queryPath("/v1/assets", { skip: 0, take: 100 }));
        if (active) setAssets(assetData);
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Varlıklar yüklenemedi.");
      }
    })();
    return () => { active = false; };
  }, []);

  async function create(event: React.FormEvent) {
    event.preventDefault();
    setSaving(true);
    try {
      await apiFetch("/v1/work-orders", { method: "POST", body: JSON.stringify({ assetId, description }) });
      setNotice("İş emri oluşturuldu.");
      setModal(null);
      setAssetId("");
      setDescription("");
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "İş emri oluşturulamadı.");
    } finally {
      setSaving(false);
    }
  }

  async function updateStatus(event: React.FormEvent) {
    event.preventDefault();
    if (!selected) return;
    setSaving(true);
    try {
      await apiFetch(`/v1/work-orders/${selected.id}/status`, { method: "PATCH", body: JSON.stringify({ id: selected.id, newStatus, permitApproved, eSignatureCaptured }) });
      setNotice("İş emri durumu güncellendi.");
      setModal(null);
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Durum güncellenemedi.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <>
      <PageHeader eyebrow="Bakım operasyonu" title="İş Emirleri" description="Bakım taleplerini durum akışıyla yönetin; izin ve e-imza gereksinimlerini takip edin." action={<button className="primary-button" onClick={() => { setAssetId(""); setDescription(""); setModal("create"); }}>+ Yeni iş emri</button>} />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <div className="card overflow-hidden">
        {loading ? <div className="flex justify-center py-16 text-teal"><Spinner /></div> : items.length === 0 ? <EmptyState label="Henüz iş emri bulunmuyor." /> : <div className="overflow-x-auto"><table className="w-full text-left text-sm"><thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500"><tr><th className="px-5 py-3">Açıklama</th><th className="px-5 py-3">Varlık ID</th><th className="px-5 py-3">Durum</th><th className="px-5 py-3" /></tr></thead><tbody className="divide-y divide-slate-100">{items.map((item) => <tr key={item.id} className="hover:bg-slate-50"><td className="px-5 py-4 font-semibold text-ink"><Link href={`/work-orders/${item.id}`} className="hover:text-teal">{item.description}</Link></td><td className="max-w-xs truncate px-5 py-4 text-xs text-slate-500">{item.assetId}</td><td className="px-5 py-4"><StatusPill value={item.status} /></td><td className="px-5 py-4 text-right"><button className="text-xs font-bold text-teal hover:underline" onClick={() => { setSelected(item); setNewStatus(item.status); setPermitApproved(false); setESignatureCaptured(false); setModal("status"); }}>Durum güncelle</button></td></tr>)}</tbody></table></div>}
        <div className="flex items-center justify-between border-t border-slate-100 px-5 py-4"><button className="secondary-button text-xs" disabled={skip === 0} onClick={() => setSkip(Math.max(0, skip - 12))}>← Önceki</button><span className="text-xs text-slate-400">Sayfa {skip / 12 + 1}</span><button className="secondary-button text-xs" disabled={items.length < 12} onClick={() => setSkip(skip + 12)}>Sonraki →</button></div>
      </div>
      {modal === "create" && <Modal title="Yeni iş emri" onClose={() => { setModal(null); setAssetId(""); setDescription(""); }}><form className="space-y-5" onSubmit={create}><label className="block text-sm font-semibold text-ink">Varlık<select className="field mt-2" required value={assetId} onChange={(event) => setAssetId(event.target.value)}><option value="">Varlık seçin</option>{assets.map((asset) => <option key={asset.id} value={asset.id}>{asset.name} · {asset.id.slice(0, 8)}</option>)}</select></label><label className="block text-sm font-semibold text-ink">Açıklama<textarea className="field mt-2 min-h-28" required value={description} onChange={(event) => setDescription(event.target.value)} placeholder="Arıza, planlı bakım veya talep açıklaması" /></label><div className="flex justify-end gap-3"><button type="button" className="secondary-button" onClick={() => { setModal(null); setAssetId(""); setDescription(""); }}>Vazgeç</button><button className="primary-button" disabled={saving}>{saving && <Spinner />} Oluştur</button></div></form></Modal>}
      {modal === "status" && selected && <Modal title="İş emri durumunu güncelle" onClose={() => setModal(null)}><form className="space-y-5" onSubmit={updateStatus}><p className="rounded-xl bg-slate-50 p-3 text-sm text-slate-600">{selected.description}</p><label className="block text-sm font-semibold text-ink">Yeni durum<select className="field mt-2" value={newStatus} onChange={(event) => setNewStatus(event.target.value)}>{statuses.map((status) => <option key={status}>{status}</option>)}</select></label><label className="flex items-center gap-2 text-sm text-slate-600"><input type="checkbox" checked={permitApproved} onChange={(event) => setPermitApproved(event.target.checked)} /> Permit onaylandı</label><label className="flex items-center gap-2 text-sm text-slate-600"><input type="checkbox" checked={eSignatureCaptured} onChange={(event) => setESignatureCaptured(event.target.checked)} /> E-imza alındı</label><div className="flex justify-end gap-3"><button type="button" className="secondary-button" onClick={() => setModal(null)}>Vazgeç</button><button className="primary-button" disabled={saving}>{saving && <Spinner />} Güncelle</button></div></form></Modal>}
    </>
  );
}
