"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch, queryPath } from "../../lib/api";
import type { Asset, InventoryTransaction, Part, Vendor, WorkOrder, Worker } from "../../lib/types";
import { EmptyState, ErrorNotice, PageHeader, Spinner, StatusPill } from "../../components/Ui";

type Summary = { label: string; value: number; href: string; accent: string };

export default function DashboardPage() {
  const [summary, setSummary] = useState<Summary[]>([]);
  const [transactions, setTransactions] = useState<InventoryTransaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function load() {
      try {
        const [workOrders, assets, parts, workers, vendors, movement] = await Promise.all([
          apiFetch<WorkOrder[]>(queryPath("/v1/work-orders", { skip: 0, take: 100 })),
          apiFetch<Asset[]>(queryPath("/v1/assets", { skip: 0, take: 100 })),
          apiFetch<Part[]>(queryPath("/v1/parts", { skip: 0, take: 100 })),
          apiFetch<Worker[]>(queryPath("/v1/workers", { skip: 0, take: 100 })),
          apiFetch<Vendor[]>(queryPath("/v1/vendors", { skip: 0, take: 100 })),
          apiFetch<InventoryTransaction[]>(queryPath("/v1/inventory/transactions", { skip: 0, take: 6 })),
        ]);
        setSummary([
          { label: "Açık iş emri", value: workOrders.filter((item) => !["Closed", "Completed"].includes(item.status)).length, href: "/work-orders", accent: "bg-blue-50 text-blue-700" },
          { label: "Varlık", value: assets.length, href: "/assets", accent: "bg-violet-50 text-violet-700" },
          { label: "Aktif parça", value: parts.filter((item) => item.isActive).length, href: "/inventory", accent: "bg-emerald-50 text-emerald-700" },
          { label: "Personel", value: workers.filter((item) => item.isActive).length, href: "/workers", accent: "bg-amber-50 text-amber-700" },
          { label: "Tedarikçi", value: vendors.filter((item) => item.isActive).length, href: "/vendors", accent: "bg-rose-50 text-rose-700" },
        ]);
        setTransactions(movement);
      } catch (cause) {
        setError(cause instanceof Error ? cause.message : "Dashboard verileri yüklenemedi.");
      } finally {
        setLoading(false);
      }
    }
    void load();
  }, []);

  return (
    <>
      <PageHeader eyebrow="Genel bakış" title="Günaydın, operasyon ekibi." description="BingehOS ile bakım performansınızı ve kritik hareketleri tek ekranda izleyin." action={<Link href="/work-orders" className="primary-button">İş emri oluştur</Link>} />
      {error && <div className="mb-5"><ErrorNotice message={error} /></div>}
      {loading ? <div className="flex justify-center py-20 text-teal"><Spinner /></div> : (
        <>
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-5">
            {summary.map((item) => <Link href={item.href} key={item.label} className="card p-5 transition hover:-translate-y-1 hover:shadow-xl"><div className={`mb-7 inline-flex rounded-xl px-3 py-1.5 text-xs font-bold ${item.accent}`}>{item.label}</div><p className="text-4xl font-black text-ink">{item.value}</p><p className="mt-2 text-xs text-slate-400">Detayları görüntüle →</p></Link>)}
          </div>
          <div className="mt-8 grid gap-6 xl:grid-cols-[1.3fr_0.7fr]">
            <section className="card overflow-hidden">
              <div className="flex items-center justify-between border-b border-slate-100 px-5 py-4"><div><h2 className="font-black text-ink">Son stok hareketleri</h2><p className="mt-1 text-xs text-slate-400">Envanter kayıt defterindeki son işlemler</p></div><Link href="/inventory" className="text-xs font-bold text-teal">Tümünü gör →</Link></div>
              {transactions.length === 0 ? <EmptyState label="Henüz stok hareketi bulunmuyor." /> : <div className="divide-y divide-slate-100">{transactions.map((tx) => <div className="flex items-center justify-between gap-4 px-5 py-4" key={tx.id}><div><p className="text-sm font-bold text-ink">{tx.partName}</p><p className="mt-1 text-xs text-slate-400">{new Date(tx.transactionDate).toLocaleString("tr-TR")}</p></div><div className="text-right"><StatusPill value={tx.type} /><p className="mt-1 text-sm font-black text-ink">{tx.quantity} {tx.unitOfMeasure}</p></div></div>)}</div>}
            </section>
            <section className="rounded-2xl bg-ink p-6 text-white shadow-soft"><p className="text-xs font-bold uppercase tracking-[0.2em] text-teal">Operasyon ipucu</p><h2 className="mt-5 text-2xl font-black leading-tight">Kritik varlıklarınızı günlük gözden geçirin.</h2><p className="mt-4 text-sm leading-6 text-slate-300">İş emirlerini durumlarına göre takip edin; stok hareketlerini ilgili iş emrine bağlayarak denetim izini güçlendirin.</p><Link href="/assets" className="mt-7 inline-block rounded-xl bg-white px-4 py-2.5 text-sm font-bold text-ink">Varlıkları aç</Link></section>
          </div>
        </>
      )}
    </>
  );
}
