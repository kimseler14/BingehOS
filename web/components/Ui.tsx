"use client";

import { useEffect } from "react";

export function PageHeader({
  eyebrow,
  title,
  description,
  action,
}: {
  eyebrow?: string;
  title: string;
  description?: string;
  action?: React.ReactNode;
}) {
  return (
    <div className="mb-7 flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
      <div>
        {eyebrow && <p className="mb-2 text-xs font-bold uppercase tracking-[0.2em] text-teal">{eyebrow}</p>}
        <h1 className="text-3xl font-black tracking-tight text-ink">{title}</h1>
        {description && <p className="mt-2 max-w-2xl text-sm leading-6 text-slate-500">{description}</p>}
      </div>
      {action}
    </div>
  );
}

export function EmptyState({ label = "Henüz kayıt bulunmuyor." }: { label?: string }) {
  return <div className="py-14 text-center text-sm text-slate-400">{label}</div>;
}

export function Spinner() {
  return <span className="inline-block h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />;
}

export function Modal({
  title,
  children,
  onClose,
}: {
  title: string;
  children: React.ReactNode;
  onClose: () => void;
}) {
  useEffect(() => {
    const onKey = (event: KeyboardEvent) => event.key === "Escape" && onClose();
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [onClose]);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-ink/50 p-4" onMouseDown={onClose}>
      <div className="max-h-[90vh] w-full max-w-xl overflow-y-auto rounded-2xl bg-white p-6 shadow-2xl" onMouseDown={(event) => event.stopPropagation()}>
        <div className="mb-5 flex items-center justify-between">
          <h2 className="text-xl font-black text-ink">{title}</h2>
          <button className="text-2xl leading-none text-slate-400 hover:text-ink" onClick={onClose}>×</button>
        </div>
        {children}
      </div>
    </div>
  );
}

export function ErrorNotice({ message }: { message: string }) {
  return <div className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-700">{message}</div>;
}

export function StatusPill({ value }: { value: string | boolean }) {
  const text = typeof value === "boolean" ? (value ? "Aktif" : "Pasif") : value;
  const positive = ["Active", "Aktif", "Completed", "Verified", "Closed", "Approved", "InProgress"].includes(text);
  return <span className={`rounded-full px-2.5 py-1 text-xs font-bold ${positive ? "bg-emerald-50 text-emerald-700" : "bg-slate-100 text-slate-600"}`}>{text}</span>;
}
