"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch } from "../lib/api";
import { EmptyState, ErrorNotice, PageHeader, Spinner, StatusPill } from "./Ui";

export function EntityDetailPage({
  title,
  eyebrow,
  endpoint,
  backHref,
  labels,
}: {
  title: string;
  eyebrow: string;
  endpoint: string;
  backHref: string;
  labels: Record<string, string>;
}) {
  const [item, setItem] = useState<Record<string, unknown> | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    void apiFetch<Record<string, unknown>>(endpoint).then(setItem).catch((cause) => setError(cause instanceof Error ? cause.message : "Detay yüklenemedi."));
  }, [endpoint]);

  if (error) return <ErrorNotice message={error} />;
  if (!item) return <div className="flex justify-center py-20 text-teal"><Spinner /></div>;

  return (
    <>
      <PageHeader eyebrow={eyebrow} title={title} action={<Link href={backHref} className="secondary-button">← Listeye dön</Link>} />
      <div className="card grid gap-5 p-6 sm:grid-cols-2">
        {Object.entries(item).map(([key, value]) => (
          <div key={key} className="border-b border-slate-100 pb-4">
            <p className="text-xs font-bold uppercase tracking-wider text-slate-400">{labels[key] || key}</p>
            <div className="mt-2 text-sm font-semibold text-ink">{typeof value === "boolean" ? <StatusPill value={value} /> : Array.isArray(value) ? value.join(", ") || "—" : value ? String(value) : "—"}</div>
          </div>
        ))}
      </div>
    </>
  );
}
