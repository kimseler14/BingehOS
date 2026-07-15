"use client";

import { useEffect, useState } from "react";
import { apiFetch } from "../lib/api";
import type { PluginRegistration } from "../lib/types";
import { EmptyState, ErrorNotice, Modal, PageHeader, Spinner, StatusPill } from "./Ui";

type PluginForm = {
  name: string;
  version: string;
  description: string;
  author: string;
  sourceUrl: string;
  storagePath: string;
};

const emptyForm: PluginForm = {
  name: "",
  version: "1.0.0",
  description: "",
  author: "",
  sourceUrl: "",
  storagePath: "",
};

const statusLabel: Record<string, string> = {
  Available: "Hazır",
  Enabled: "Etkin",
  Disabled: "Devre dışı",
};

export function PluginsPage() {
  const [plugins, setPlugins] = useState<PluginRegistration[]>([]);
  const [form, setForm] = useState<PluginForm>(emptyForm);
  const [modal, setModal] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");

  async function load() {
    setLoading(true);
    try {
      setPlugins(await apiFetch<PluginRegistration[]>("/v1/plugins?skip=0&take=100"));
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Eklentiler yüklenemedi.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const data = await apiFetch<PluginRegistration[]>("/v1/plugins?skip=0&take=100");
        if (active) setPlugins(data);
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Eklentiler yüklenemedi.");
      } finally {
        if (active) setLoading(false);
      }
    })();
    return () => {
      active = false;
    };
  }, []);

  async function register(event: React.FormEvent) {
    event.preventDefault();
    setSaving(true);
    setError("");
    try {
      await apiFetch("/v1/plugins", {
        method: "POST",
        body: JSON.stringify({
          ...form,
          description: form.description || null,
          author: form.author || null,
          sourceUrl: form.sourceUrl || null,
          storagePath: form.storagePath || null,
        }),
      });
      setModal(false);
      setForm(emptyForm);
      setNotice("Eklenti kaydı oluşturuldu.");
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Eklenti kaydedilemedi.");
    } finally {
      setSaving(false);
    }
  }

  async function setStatus(plugin: PluginRegistration, status: string) {
    setError("");
    try {
      await apiFetch(`/v1/plugins/${plugin.id}`, {
        method: "PATCH",
        body: JSON.stringify({
          id: plugin.id,
          name: plugin.name,
          version: plugin.version,
          description: plugin.description,
          author: plugin.author,
          status,
          sourceUrl: plugin.sourceUrl,
          storagePath: plugin.storagePath,
        }),
      });
      setNotice(status === "Enabled" ? "Eklenti etkinleştirildi." : "Eklenti devre dışı bırakıldı.");
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Eklenti durumu değiştirilemedi.");
    }
  }

  async function remove(plugin: PluginRegistration) {
    if (!window.confirm(`“${plugin.name}” eklentisi silinsin mi?`)) return;
    try {
      await apiFetch(`/v1/plugins/${plugin.id}`, { method: "DELETE" });
      setNotice("Eklenti silindi.");
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Eklenti silinemedi.");
    }
  }

  return (
    <>
      <PageHeader
        eyebrow="Marketplace yönetimi"
        title="Eklentiler"
        description="Kiracınız için kayıtlı eklentileri yönetin. Başlangıç sürümünde eklenti dosyaları yönetim metadatası olarak tutulur."
        action={<button className="primary-button" onClick={() => setModal(true)}>+ Eklenti kaydet</button>}
      />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <div className="card overflow-hidden">
        {loading ? <div className="flex justify-center py-16 text-teal"><Spinner /></div> : plugins.length === 0 ? <EmptyState label="Henüz kayıtlı eklenti bulunmuyor." /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500"><tr><th className="px-5 py-3">Eklenti</th><th className="px-5 py-3">Yazar</th><th className="px-5 py-3">Durum</th><th className="px-5 py-3" /></tr></thead>
              <tbody className="divide-y divide-slate-100">
                {plugins.map((plugin) => (
                  <tr key={plugin.id}>
                    <td className="px-5 py-4"><p className="font-bold text-ink">{plugin.name} <span className="text-xs font-normal text-slate-400">v{plugin.version}</span></p><p className="mt-1 text-xs text-slate-400">{plugin.description || "Açıklama yok"}</p></td>
                    <td className="px-5 py-4 text-slate-600">{plugin.author || "—"}</td>
                    <td className="px-5 py-4"><StatusPill value={statusLabel[plugin.status] ?? plugin.status} /></td>
                    <td className="whitespace-nowrap px-5 py-4 text-right">
                      <button className="mr-3 text-xs font-bold text-teal hover:underline" onClick={() => void setStatus(plugin, plugin.status === "Enabled" ? "Disabled" : "Enabled")}>{plugin.status === "Enabled" ? "Devre dışı" : "Etkinleştir"}</button>
                      <button className="text-xs font-bold text-rose-600 hover:underline" onClick={() => void remove(plugin)}>Sil</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
      {modal && <Modal title="Eklenti kaydet" onClose={() => setModal(false)}>
        <form className="space-y-4" onSubmit={register}>
          <label className="block text-sm font-semibold text-ink">Ad<input className="field mt-2" required value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} /></label>
          <div className="grid gap-4 sm:grid-cols-2"><label className="block text-sm font-semibold text-ink">Sürüm<input className="field mt-2" required value={form.version} onChange={(event) => setForm({ ...form, version: event.target.value })} /></label><label className="block text-sm font-semibold text-ink">Yazar<input className="field mt-2" value={form.author} onChange={(event) => setForm({ ...form, author: event.target.value })} /></label></div>
          <label className="block text-sm font-semibold text-ink">Açıklama<textarea className="field mt-2" value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} /></label>
          <label className="block text-sm font-semibold text-ink">Kaynak URL<input className="field mt-2" type="url" value={form.sourceUrl} onChange={(event) => setForm({ ...form, sourceUrl: event.target.value })} /></label>
          <label className="block text-sm font-semibold text-ink">Depolama yolu<input className="field mt-2" value={form.storagePath} onChange={(event) => setForm({ ...form, storagePath: event.target.value })} placeholder="/plugins/example" /></label>
          <div className="flex justify-end gap-3"><button type="button" className="secondary-button" onClick={() => setModal(false)}>Vazgeç</button><button className="primary-button" disabled={saving}>{saving && <Spinner />} Kaydet</button></div>
        </form>
      </Modal>}
    </>
  );
}
