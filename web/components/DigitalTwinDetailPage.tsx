"use client";

import { useCallback, useEffect, useState } from "react";
import Link from "next/link";
import { apiFetch } from "../lib/api";
import type { AssetPosition, FloorPlan } from "../lib/types";
import { ErrorNotice, PageHeader, Spinner, StatusPill } from "./Ui";

function markerColor(criticality: string) {
  return criticality === "Critical" ? "bg-rose-600" : criticality === "High" ? "bg-amber-500" : criticality === "Low" ? "bg-slate-500" : "bg-teal";
}

export function DigitalTwinDetailPage({ id }: { id: string }) {
  const [plan, setPlan] = useState<FloorPlan | null>(null);
  const [positions, setPositions] = useState<AssetPosition[]>([]);
  const [selected, setSelected] = useState<AssetPosition | null>(null);
  const [editing, setEditing] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");
  const [dragging, setDragging] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [planData, positionData] = await Promise.all([
        apiFetch<FloorPlan>(`/v1/floor-plans/${id}`),
        apiFetch<AssetPosition[]>(`/v1/floor-plans/${id}/positions`),
      ]);
      setPlan(planData);
      setPositions(positionData);
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Dijital ikiz verisi yüklenemedi.");
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const [planData, positionData] = await Promise.all([
          apiFetch<FloorPlan>(`/v1/floor-plans/${id}`),
          apiFetch<AssetPosition[]>(`/v1/floor-plans/${id}/positions`),
        ]);
        if (active) {
          setPlan(planData);
          setPositions(positionData);
        }
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Dijital ikiz verisi yüklenemedi.");
      } finally {
        if (active) setLoading(false);
      }
    })();
    return () => {
      active = false;
    };
  }, [id]);

  function moveMarker(event: React.PointerEvent<HTMLElement>, assetId: string) {
    if (!editing) return;
    const bounds = event.currentTarget.parentElement?.getBoundingClientRect();
    if (!bounds) return;
    const x = Math.max(0, Math.min(1, (event.clientX - bounds.left) / bounds.width));
    const y = Math.max(0, Math.min(1, (event.clientY - bounds.top) / bounds.height));
    setPositions((current) => current.map((position) => position.assetId === assetId ? { ...position, x, y } : position));
  }

  async function savePositions() {
    setSaving(true);
    setError("");
    try {
      await apiFetch(`/v1/floor-plans/${id}/positions`, {
        method: "PUT",
        body: JSON.stringify({ positions: positions.map((position) => ({ assetId: position.assetId, x: position.x, y: position.y })) }),
      });
      setEditing(false);
      setNotice("Varlık konumları kaydedildi.");
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Konumlar kaydedilemedi.");
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <div className="flex justify-center py-16 text-teal"><Spinner /></div>;
  if (!plan) return <ErrorNotice message={error || "Kat planı bulunamadı."} />;

  return (
    <>
      <PageHeader eyebrow="Dijital İkiz" title={plan.name} description={`${plan.width} × ${plan.height} px · ${positions.length} konumlu varlık`} action={editing ? <button className="primary-button" disabled={saving} onClick={() => void savePositions()}>{saving && <Spinner />} Konumları kaydet</button> : <button className="secondary-button" onClick={() => setEditing(true)}>Düzenle</button>} />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_280px]">
        <div className="card p-4"><div className="relative mx-auto aspect-[3/2] max-h-[70vh] overflow-hidden rounded-xl border-2 border-slate-200 bg-slate-50" style={{ aspectRatio: `${plan.width} / ${plan.height}`, backgroundImage: plan.imageUrl ? `url(${plan.imageUrl})` : undefined, backgroundSize: "cover", backgroundPosition: "center" }} onPointerMove={(event) => dragging && moveMarker(event, dragging)} onPointerUp={() => setDragging(null)}>
          {!plan.imageUrl && <div className="pointer-events-none absolute inset-0 opacity-40" style={{ backgroundImage: "linear-gradient(#94a3b8 1px, transparent 1px), linear-gradient(90deg, #94a3b8 1px, transparent 1px)", backgroundSize: "10% 10%" }} />}
          {positions.map((position) => <button key={position.id} type="button" className={`absolute z-10 h-5 w-5 -translate-x-1/2 -translate-y-1/2 rounded-full border-2 border-white shadow-lg ${markerColor(position.criticality)} ${editing ? "cursor-move ring-4 ring-teal/20" : ""}`} style={{ left: `${position.x * 100}%`, top: `${position.y * 100}%` }} onPointerDown={(event) => { if (editing) { event.currentTarget.setPointerCapture(event.pointerId); setDragging(position.assetId); } }} onPointerMove={(event) => dragging === position.assetId && moveMarker(event, position.assetId)} onClick={() => setSelected(position)} aria-label={position.assetName} />)}
        </div></div>
        <aside className="card p-5"><h2 className="font-black text-ink">Varlıklar</h2><p className="mt-1 text-xs text-slate-500">{editing ? "İşaretçileri sürükleyerek konumlandırın." : "Bir işaretçiye tıklayarak ayrıntıyı görün."}</p><div className="mt-4 space-y-3">{positions.length === 0 ? <p className="text-sm text-slate-400">Bu planda henüz varlık yok.</p> : positions.map((position) => <button type="button" className="flex w-full items-center gap-3 rounded-xl border border-slate-100 p-3 text-left hover:bg-slate-50" key={position.id} onClick={() => setSelected(position)}><span className={`h-3 w-3 shrink-0 rounded-full ${markerColor(position.criticality)}`} /><span className="min-w-0 flex-1 truncate text-sm font-semibold text-ink">{position.assetName}</span><StatusPill value={position.criticality} /></button>)}</div></aside>
      </div>
      {selected && <div className="fixed bottom-6 right-6 z-20 w-80 rounded-2xl border border-slate-200 bg-white p-5 shadow-2xl"><div className="flex items-start justify-between"><div><p className="text-xs font-bold uppercase tracking-wider text-teal">Varlık</p><h2 className="mt-1 font-black text-ink">{selected.assetName}</h2></div><button className="text-xl text-slate-400" onClick={() => setSelected(null)}>×</button></div><div className="mt-4 flex items-center justify-between text-sm"><span className="text-slate-500">Kritiklik</span><StatusPill value={selected.criticality} /></div><p className="mt-3 text-xs text-slate-500">Konum: %{(selected.x * 100).toFixed(1)} yatay, %{(selected.y * 100).toFixed(1)} dikey</p><Link className="mt-4 inline-block text-sm font-bold text-teal hover:underline" href={`/assets/${selected.assetId}`}>Varlık detayına git →</Link></div>}
    </>
  );
}
