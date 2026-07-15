"use client";

import { useEffect, useState } from "react";
import { apiFetch } from "../lib/api";
import type { AssetMaintenanceInsight, PartReorderInsight } from "../lib/types";
import { EmptyState, ErrorNotice, PageHeader, Spinner, StatusPill } from "./Ui";

export function InsightsPage() {
  const [assets, setAssets] = useState<AssetMaintenanceInsight[]>([]);
  const [parts, setParts] = useState<PartReorderInsight[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const [assetData, partData] = await Promise.all([
          apiFetch<AssetMaintenanceInsight[]>("/v1/insights/assets"),
          apiFetch<PartReorderInsight[]>("/v1/insights/parts"),
        ]);
        if (active) {
          setAssets(assetData);
          setParts(partData);
        }
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Analiz verileri yüklenemedi.");
      } finally {
        if (active) setLoading(false);
      }
    })();
    return () => {
      active = false;
    };
  }, []);

  return (
    <>
      <PageHeader
        eyebrow="İstatistiksel karar desteği"
        title="Analiz / Öngörüler"
        description="Bakım geçmişi ve stok hareketlerinden hesaplanan, harici yapay zekâ servisi kullanmayan operasyonel göstergeler."
      />
      {error && <div className="mb-5"><ErrorNotice message={error} /></div>}
      {loading ? <div className="flex justify-center py-16 text-teal"><Spinner /></div> : (
        <div className="space-y-6">
          <section className="card overflow-hidden">
            <div className="border-b border-slate-100 px-5 py-4"><h2 className="font-black text-ink">Varlık bakım riskleri</h2><p className="mt-1 text-xs text-slate-500">Düzeltici iş emirlerinin sıklığı ve son 90 günlük değişimi.</p></div>
            {assets.length === 0 ? <EmptyState label="Varlık bakım geçmişi bulunmuyor." /> : <div className="overflow-x-auto"><table className="w-full text-left text-sm"><thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500"><tr><th className="px-5 py-3">Varlık</th><th className="px-5 py-3">Risk</th><th className="px-5 py-3">Sıklık / ay</th><th className="px-5 py-3">MTBF</th><th className="px-5 py-3">Gerekçe</th></tr></thead><tbody className="divide-y divide-slate-100">{assets.map((asset) => <tr key={asset.assetId}><td className="px-5 py-4 font-bold text-ink">{asset.assetName}<p className="mt-1 text-xs font-normal text-slate-400">{asset.failureCount} düzeltici iş emri · trend: {asset.trend === "Elevated" ? "yükseliyor" : asset.trend === "Improving" ? "iyileşiyor" : "sabit"}</p></td><td className="px-5 py-4"><StatusPill value={asset.risk === "High" ? "Yüksek" : asset.risk === "Medium" ? "Orta" : "Düşük"} /></td><td className="px-5 py-4">{asset.failureFrequencyPerMonth.toFixed(1)}</td><td className="px-5 py-4">{asset.meanTimeBetweenFailuresDays ? `${asset.meanTimeBetweenFailuresDays.toFixed(0)} gün` : "—"}</td><td className="max-w-md px-5 py-4 text-xs leading-5 text-slate-600">{asset.rationale}</td></tr>)}</tbody></table></div>}
          </section>
          <section className="card overflow-hidden">
            <div className="border-b border-slate-100 px-5 py-4"><h2 className="font-black text-ink">Yeniden sipariş önerileri</h2><p className="mt-1 text-xs text-slate-500">Son 6 aylık kullanım hızına göre mevcut stok eşiğin altındaysa gösterilir.</p></div>
            {parts.length === 0 ? <EmptyState label="Eşik altında parça bulunmuyor." /> : <div className="overflow-x-auto"><table className="w-full text-left text-sm"><thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500"><tr><th className="px-5 py-3">Parça</th><th className="px-5 py-3">Mevcut</th><th className="px-5 py-3">Önerilen eşik</th><th className="px-5 py-3">Aylık kullanım</th><th className="px-5 py-3">Gerekçe</th></tr></thead><tbody className="divide-y divide-slate-100">{parts.map((part) => <tr key={part.partId}><td className="px-5 py-4"><p className="font-bold text-ink">{part.partName}</p><p className="text-xs text-slate-400">{part.partNumber}</p></td><td className="px-5 py-4 font-bold text-rose-600">{part.currentStock}</td><td className="px-5 py-4">{part.suggestedReorderThreshold}</td><td className="px-5 py-4">{part.averageMonthlyIssueQuantity.toFixed(1)}</td><td className="max-w-md px-5 py-4 text-xs leading-5 text-slate-600">{part.rationale}</td></tr>)}</tbody></table></div>}
          </section>
        </div>
      )}
    </>
  );
}
