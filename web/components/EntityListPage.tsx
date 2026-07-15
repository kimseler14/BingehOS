"use client";

import { Suspense, useCallback, useEffect, useMemo, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { apiFetch, queryPath } from "../lib/api";
import { EmptyState, ErrorNotice, Modal, PageHeader, Spinner, StatusPill } from "./Ui";
import { EntityDetailPage } from "./EntityDetailPage";

export type FieldConfig = {
  name: string;
  label: string;
  type?: "text" | "number" | "checkbox" | "textarea" | "select";
  required?: boolean;
  placeholder?: string;
  options?: string[];
  defaultValue?: boolean;
};

export type ColumnConfig = {
  key: string;
  label: string;
  type?: "status" | "date";
};

export type DetailConfig = {
  title: string;
  eyebrow: string;
  endpoint: (id: string) => string;
  backHref: string;
  labels: Record<string, string>;
};

export type EntityConfig = {
  title: string;
  eyebrow: string;
  description: string;
  endpoint: string;
  basePath: string;
  singular: string;
  columns: ColumnConfig[];
  createFields: FieldConfig[];
  editFields?: FieldConfig[];
  activeFilter?: boolean;
  deletable?: boolean;
  canCreate?: boolean;
  canDelete?: (item: Record<string, unknown>) => boolean;
  detail?: DetailConfig;
};

type FormValue = string | number | boolean;

function emptyValues(fields: FieldConfig[]) {
  return fields.reduce<Record<string, FormValue>>((values, field) => {
    values[field.name] = field.type === "checkbox" ? field.defaultValue ?? false : "";
    return values;
  }, {});
}

function FormFields({
  fields,
  values,
  onChange,
}: {
  fields: FieldConfig[];
  values: Record<string, FormValue>;
  onChange: (name: string, value: FormValue) => void;
}) {
  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {fields.map((field) => (
        <label key={field.name} className={`block text-sm font-semibold text-ink ${field.type === "textarea" ? "sm:col-span-2" : ""}`}>
          {field.label}
          {field.type === "checkbox" ? (
            <span className="mt-2 flex items-center gap-2 font-normal text-slate-600">
              <input type="checkbox" checked={Boolean(values[field.name])} onChange={(event) => onChange(field.name, event.target.checked)} />
              Aktif
            </span>
          ) : field.type === "textarea" ? (
            <textarea className="field mt-2 min-h-24 resize-y" required={field.required} placeholder={field.placeholder} value={String(values[field.name] ?? "")} onChange={(event) => onChange(field.name, event.target.value)} />
          ) : field.type === "select" ? (
            <select className="field mt-2" required={field.required} value={String(values[field.name] ?? "")} onChange={(event) => onChange(field.name, event.target.value)}>
              <option value="">Seçiniz</option>
              {field.options?.map((option) => <option key={option} value={option}>{option}</option>)}
            </select>
          ) : (
            <input className="field mt-2" type={field.type === "number" ? "number" : "text"} required={field.required} placeholder={field.placeholder} value={String(values[field.name] ?? "")} onChange={(event) => onChange(field.name, field.type === "number" ? Number(event.target.value) : event.target.value)} />
          )}
        </label>
      ))}
    </div>
  );
}

function cleanPayload(values: Record<string, FormValue>) {
  return Object.fromEntries(Object.entries(values).filter(([, value]) => value !== "" && !(typeof value === "number" && !Number.isFinite(value))));
}

export function EntityListPage({ config }: { config: EntityConfig }) {
  return (
    <Suspense fallback={<div className="flex justify-center py-20 text-teal"><Spinner /></div>}>
      <EntityListPageInner config={config} />
    </Suspense>
  );
}

function EntityListPageInner({ config }: { config: EntityConfig }) {
  const router = useRouter();
  const searchParams = useSearchParams();
  const id = searchParams.get("id");

  if (id && config.detail) {
    return <EntityDetailPage title={config.detail.title} eyebrow={config.detail.eyebrow} endpoint={config.detail.endpoint(id)} backHref={config.detail.backHref} labels={config.detail.labels} />;
  }
  const [items, setItems] = useState<Record<string, unknown>[]>([]);
  const [skip, setSkip] = useState(0);
  const [activeOnly, setActiveOnly] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");
  const [modal, setModal] = useState<"create" | "edit" | null>(null);
  const [selected, setSelected] = useState<Record<string, unknown> | null>(null);
  const [values, setValues] = useState<Record<string, FormValue>>(emptyValues(config.createFields));
  const fields = modal === "edit" && config.editFields ? config.editFields : config.createFields;

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const data = await apiFetch<Record<string, unknown>[]>(
        queryPath(config.endpoint, { skip, take: 12, ...(config.activeFilter ? { activeOnly: activeOnly || undefined } : {}) }),
      );
      setItems(Array.isArray(data) ? data : []);
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Kayıtlar yüklenemedi.");
    } finally {
      setLoading(false);
    }
  }, [activeOnly, config.activeFilter, config.endpoint, skip]);

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const data = await apiFetch<Record<string, unknown>[]>(queryPath(config.endpoint, { skip, take: 12, activeOnly: config.activeFilter ? activeOnly : undefined }));
        if (active) setItems(data);
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Kayıtlar yüklenemedi.");
      } finally {
        if (active) setLoading(false);
      }
    })();
    return () => { active = false; };
  }, [activeOnly, config.activeFilter, config.endpoint, skip]);

  const openCreate = () => {
    setValues(emptyValues(config.createFields));
    setSelected(null);
    setModal("create");
    setError("");
  };

  const openEdit = (item: Record<string, unknown>) => {
    const editFields = config.editFields || config.createFields;
    setValues(editFields.reduce<Record<string, FormValue>>((result, field) => {
      result[field.name] = (item[field.name] as FormValue | undefined) ?? (field.type === "checkbox" ? field.defaultValue ?? false : "");
      return result;
    }, {}));
    setSelected(item);
    setModal("edit");
    setError("");
  };

  const save = async (event: React.FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setError("");
    try {
      const payload = modal === "edit"
        ? Object.fromEntries(Object.entries(values).filter(([, value]) => !(typeof value === "number" && !Number.isFinite(value))))
        : cleanPayload(values);
      if (modal === "create") {
        await apiFetch(config.endpoint, { method: "POST", body: JSON.stringify(payload) });
        setNotice(`${config.singular} oluşturuldu.`);
      } else if (selected?.id) {
        await apiFetch(`${config.endpoint}/${selected.id}`, { method: "PATCH", body: JSON.stringify({ id: selected.id, ...payload }) });
        setNotice(`${config.singular} güncellendi.`);
      }
      setModal(null);
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Kayıt kaydedilemedi.");
    } finally {
      setSaving(false);
    }
  };

  const remove = async (item: Record<string, unknown>) => {
    if (!item.id || !window.confirm(`${config.singular} silinsin mi?`)) return;
    try {
      await apiFetch(`${config.endpoint}/${item.id}`, { method: "DELETE" });
      setNotice(`${config.singular} silindi.`);
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Kayıt silinemedi.");
    }
  };

  const columns = useMemo(() => config.columns, [config.columns]);

  return (
    <>
      <PageHeader eyebrow={config.eyebrow} title={config.title} description={config.description} action={config.canCreate !== false ? <button className="primary-button" onClick={openCreate}>+ Yeni {config.singular}</button> : undefined} />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <div className="card overflow-hidden">
        <div className="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 px-5 py-4">
          <p className="text-sm font-semibold text-slate-500">{items.length} kayıt gösteriliyor</p>
          {config.activeFilter && <label className="flex items-center gap-2 text-sm text-slate-600"><input type="checkbox" checked={activeOnly} onChange={(event) => { setActiveOnly(event.target.checked); setSkip(0); }} /> Sadece aktif</label>}
        </div>
        {loading ? <div className="flex justify-center py-16 text-teal"><Spinner /></div> : items.length === 0 ? <EmptyState /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500">
                <tr>{columns.map((column) => <th key={column.key} className="whitespace-nowrap px-5 py-3 font-bold">{column.label}</th>)}<th className="px-5 py-3" /></tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {items.map((item) => (
                  <tr key={String(item.id)} className="group cursor-pointer hover:bg-slate-50" onClick={() => router.push(`${config.basePath}?id=${item.id}`)}>
                    {columns.map((column) => {
                      const value = item[column.key];
                      return <td key={column.key} className="max-w-xs truncate px-5 py-4 text-slate-600">{column.type === "status" ? <StatusPill value={value as string | boolean} /> : column.type === "date" ? new Date(String(value)).toLocaleDateString("tr-TR") : String(value ?? "—")}</td>;
                    })}
                    <td className="whitespace-nowrap px-5 py-4 text-right" onClick={(event) => event.stopPropagation()}>
                      <button className="mr-2 text-xs font-bold text-teal hover:underline" onClick={() => openEdit(item)}>Düzenle</button>
                      {config.deletable && config.canDelete?.(item) !== false && <button className="text-xs font-bold text-rose-600 hover:underline" onClick={() => void remove(item)}>Sil</button>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        <div className="flex items-center justify-between border-t border-slate-100 px-5 py-4">
          <button className="secondary-button text-xs" disabled={skip === 0} onClick={() => setSkip(Math.max(0, skip - 12))}>← Önceki</button>
          <span className="text-xs text-slate-400">Sayfa {skip / 12 + 1}</span>
          <button className="secondary-button text-xs" disabled={items.length < 12} onClick={() => setSkip(skip + 12)}>Sonraki →</button>
        </div>
      </div>
      {modal && <Modal title={`${modal === "create" ? "Yeni" : "Düzenle"} ${config.singular}`} onClose={() => setModal(null)}>
        <form className="space-y-5" onSubmit={save}>
          <FormFields fields={fields} values={values} onChange={(name, value) => setValues((current) => ({ ...current, [name]: value }))} />
          <div className="flex justify-end gap-3"><button type="button" className="secondary-button" onClick={() => setModal(null)}>Vazgeç</button><button className="primary-button" disabled={saving}>{saving && <Spinner />} Kaydet</button></div>
        </form>
      </Modal>}
    </>
  );
}
