"use client";

import { useEffect, useState } from "react";
import { apiFetch } from "../../lib/api";
import { ErrorNotice, PageHeader, Spinner } from "../../components/Ui";

type Meter = {
  id: string;
  assetId: string;
  name: string;
  unit: string;
  meterType?: string | null;
  lastReadingAt?: string | null;
  lastReadingValue?: number | null;
};

type EnergyCost = {
  id: string;
  assetId: string;
  billingPeriodStart: string;
  billingPeriodEnd: string;
  amountMinor: number;
  currency: string;
  energyType?: string | null;
  meterNumber?: string | null;
  provider?: string | null;
};

export default function EnergyPage() {
  const [meters, setMeters] = useState<Meter[]>([]);
  const [costs, setCosts] = useState<EnergyCost[]>([]);
  const [error, setError] = useState("");

  useEffect(() => {
    apiFetch<Meter[]>("/v1/energy/meters?assetId=00000000-0000-0000-0000-000000000000")
      .then(setMeters)
      .catch(() => {});
    apiFetch<EnergyCost[]>("/v1/energy/costs")
      .then(setCosts)
      .catch(() => {});
  }, []);

  const totalCost = costs.reduce((s, c) => s + c.amountMinor, 0);

  return (
    <main className="mx-auto max-w-6xl px-4 py-8">
      <PageHeader title="Enerji Yönetimi" eyebrow="IoT & Enerji" description="Sayaç okumaları ve enerji maliyetleri." />
      {error && <ErrorNotice message={error} />}

      <section className="mt-8 grid gap-6 sm:grid-cols-3">
        <div className="rounded-2xl bg-white p-6 shadow-sm"><p className="text-sm text-slate-500">Sayaç</p><p className="mt-1 text-3xl font-black text-ink">{meters.length}</p></div>
        <div className="rounded-2xl bg-white p-6 shadow-sm"><p className="text-sm text-slate-500">Toplam maliyet</p><p className="mt-1 text-3xl font-black text-teal">{(totalCost / 100).toFixed(2)} ₺</p></div>
        <div className="rounded-2xl bg-white p-6 shadow-sm"><p className="text-sm text-slate-500">Fatura dönemi</p><p className="mt-1 text-3xl font-black text-ink">{costs.length}</p></div>
      </section>

      <section className="mt-10">
        <h2 className="text-xl font-bold text-ink">Sayaçlar</h2>
        <div className="mt-4 overflow-x-auto rounded-2xl bg-white shadow-sm">
          {meters.length === 0 ? (
            <p className="p-6 text-sm text-slate-400">Sayaç bulunamadı.</p>
          ) : (
            <table className="w-full text-left text-sm">
              <thead><tr className="border-b text-slate-500"><th className="p-4 font-medium">Sayaç</th><th className="p-4 font-medium">Birim</th><th className="p-4 font-medium">Tür</th><th className="p-4 font-medium">Son okuma</th><th className="p-4 font-medium">Değer</th></tr></thead>
              <tbody>{meters.map((m) => (<tr key={m.id} className="border-b last:border-0"><td className="p-4 text-ink">{m.name}</td><td className="p-4">{m.unit}</td><td className="p-4">{m.meterType || "—"}</td><td className="p-4">{m.lastReadingAt ? new Date(m.lastReadingAt).toLocaleDateString("tr-TR") : "—"}</td><td className="p-4">{m.lastReadingValue ?? "—"}</td></tr>))}</tbody>
            </table>
          )}
        </div>
      </section>

      <section className="mt-10">
        <h2 className="text-xl font-bold text-ink">Enerji Maliyetleri</h2>
        <div className="mt-4 overflow-x-auto rounded-2xl bg-white shadow-sm">
          {costs.length === 0 ? (
            <p className="p-6 text-sm text-slate-400">Maliyet kaydı bulunamadı.</p>
          ) : (
            <table className="w-full text-left text-sm">
              <thead><tr className="border-b text-slate-500"><th className="p-4 font-medium">Dönem</th><th className="p-4 font-medium">Tür</th><th className="p-4 font-medium">Tedarikçi</th><th className="p-4 font-medium">Tutar</th></tr></thead>
              <tbody>{costs.map((c) => (<tr key={c.id} className="border-b last:border-0"><td className="p-4">{new Date(c.billingPeriodStart).toLocaleDateString("tr-TR")} - {new Date(c.billingPeriodEnd).toLocaleDateString("tr-TR")}</td><td className="p-4">{c.energyType || "—"}</td><td className="p-4">{c.provider || "—"}</td><td className="p-4">{(c.amountMinor / 100).toFixed(2)} {c.currency}</td></tr>))}</tbody>
            </table>
          )}
        </div>
      </section>
    </main>
  );
}
