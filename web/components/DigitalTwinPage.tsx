"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { apiFetch } from "../lib/api";
import type { FloorPlan } from "../lib/types";
import { EmptyState, ErrorNotice, Modal, PageHeader, Spinner } from "./Ui";

type FormState = {
  name: string;
  facilityId: string;
  imageUrl: string;
  width: string;
  height: string;
};

const initialForm: FormState = { name: "", facilityId: "", imageUrl: "", width: "1200", height: "800" };

export function DigitalTwinPage() {
  const [plans, setPlans] = useState<FloorPlan[]>([]);
  const [form, setForm] = useState<FormState>(initialForm);
  const [modal, setModal] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");

  async function load() {
    setLoading(true);
    try {
      setPlans(await apiFetch<FloorPlan[]>("/v1/floor-plans?skip=0&take=100"));
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Kat planları yüklenemedi.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const data = await apiFetch<FloorPlan[]>("/v1/floor-plans?skip=0&take=100");
        if (active) setPlans(data);
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Kat planları yüklenemedi.");
      } finally {
        if (active) setLoading(false);
      }
    })();
    return () => {
      active = false;
    };
  }, []);

  async function create(event: React.FormEvent) {
    event.preventDefault();
    setSaving(true);
    setError("");
    try {
      await apiFetch("/v1/floor-plans", {
        method: "POST",
        body: JSON.stringify({
          name: form.name,
          facilityId: form.facilityId || null,
          imageUrl: form.imageUrl || null,
          width: Number(form.width),
          height: Number(form.height),
        }),
      });
      setModal(false);
      setForm(initialForm);
      setNotice("Kat planı oluşturuldu.");
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Kat planı oluşturulamadı.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <>
      <PageHeader
        eyebrow="Temel varlık yerleşimi"
        title="Dijital İkiz"
        description="Kat planlarını yönetin ve varlıkları göreli koordinatlarla plan üzerine yerleştirin. BIM/IFC içe aktarma bu sürümde kapsam dışıdır."
        action={<button className="primary-button" onClick={() => setModal(true)}>+ Kat planı</button>}
      />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <div className="card overflow-hidden">
        {loading ? <div className="flex justify-center py-16 text-teal"><Spinner /></div> : plans.length === 0 ? <EmptyState label="Henüz kat planı bulunmuyor." /> : <div className="divide-y divide-slate-100">{plans.map((plan) => <Link className="flex items-center justify-between px-5 py-5 transition hover:bg-slate-50" href={`/digital-twin/${plan.id}`} key={plan.id}><div><p className="font-bold text-ink">{plan.name}</p><p className="mt-1 text-xs text-slate-500">{plan.width} × {plan.height} px · {plan.imageUrl ? "Görsel tanımlı" : "Şematik görünüm"}</p></div><span className="text-sm font-bold text-teal">Planı aç →</span></Link>)}</div>}
      </div>
      {modal && <Modal title="Kat planı oluştur" onClose={() => setModal(false)}><form className="space-y-4" onSubmit={create}>
        <label className="block text-sm font-semibold text-ink">Plan adı<input className="field mt-2" required value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} placeholder="Üretim katı" /></label>
        <label className="block text-sm font-semibold text-ink">Tesis ID (isteğe bağlı)<input className="field mt-2" value={form.facilityId} onChange={(event) => setForm({ ...form, facilityId: event.target.value })} placeholder="UUID" /></label>
        <label className="block text-sm font-semibold text-ink">Görsel URL (isteğe bağlı)<input className="field mt-2" type="url" value={form.imageUrl} onChange={(event) => setForm({ ...form, imageUrl: event.target.value })} /></label>
        <div className="grid gap-4 sm:grid-cols-2"><label className="block text-sm font-semibold text-ink">Genişlik<input className="field mt-2" type="number" min="1" required value={form.width} onChange={(event) => setForm({ ...form, width: event.target.value })} /></label><label className="block text-sm font-semibold text-ink">Yükseklik<input className="field mt-2" type="number" min="1" required value={form.height} onChange={(event) => setForm({ ...form, height: event.target.value })} /></label></div>
        <div className="flex justify-end gap-3"><button type="button" className="secondary-button" onClick={() => setModal(false)}>Vazgeç</button><button className="primary-button" disabled={saving}>{saving && <Spinner />} Oluştur</button></div>
      </form></Modal>}
    </>
  );
}
