"use client";

import { useCallback, useEffect, useState } from "react";
import { apiFetch } from "../lib/api";
import type { AutomationRule, AutomationRuleExecution } from "../lib/types";
import { ErrorNotice, EmptyState, Modal, PageHeader, Spinner, StatusPill } from "./Ui";

const triggers = [
  ["WorkOrderCreated", "İş emri oluşturuldu"],
  ["WorkOrderStatusChanged", "İş emri durumu değişti"],
  ["InventoryStockLow", "Stok seviyesi düştü"],
  ["CalibrationDue", "Kalibrasyon zamanı geldi"],
] as const;

const actions = [
  ["SendNotification", "Bildirim gönder"],
  ["CreateWorkOrder", "İş emri oluştur"],
  ["AdjustPriority", "Öncelik ayarla"],
] as const;

type FormState = {
  name: string;
  description: string;
  isEnabled: boolean;
  triggerType: string;
  conditionField: string;
  conditionOperator: string;
  conditionValue: string;
  actionType: string;
  actionMessage: string;
  actionDescription: string;
  actionPriority: string;
};

const emptyForm: FormState = {
  name: "",
  description: "",
  isEnabled: true,
  triggerType: "WorkOrderCreated",
  conditionField: "status",
  conditionOperator: "equals",
  conditionValue: "Draft",
  actionType: "SendNotification",
  actionMessage: "",
  actionDescription: "",
  actionPriority: "1",
};

function parseForm(rule: AutomationRule): FormState {
  let condition: { field?: string; operator?: string; value?: string | number } = {};
  let parameters: { message?: string; description?: string; priority?: number } = {};
  try {
    condition = JSON.parse(rule.conditionJson) as typeof condition;
  } catch {
    // Keep defaults when a legacy rule has invalid condition JSON.
  }
  try {
    parameters = JSON.parse(rule.actionParametersJson) as typeof parameters;
  } catch {
    // Keep defaults when a legacy rule has invalid action JSON.
  }
  return {
    name: rule.name,
    description: rule.description ?? "",
    isEnabled: rule.isEnabled,
    triggerType: rule.triggerType,
    conditionField: condition.field ?? "status",
    conditionOperator: condition.operator ?? "equals",
    conditionValue: String(condition.value ?? ""),
    actionType: rule.actionType,
    actionMessage: parameters.message ?? "",
    actionDescription: parameters.description ?? "",
    actionPriority: String(parameters.priority ?? 1),
  };
}

function toPayload(form: FormState) {
  const numeric = ["greater", "less"].includes(form.conditionOperator)
    ? Number(form.conditionValue)
    : form.conditionValue;
  return {
    name: form.name,
    description: form.description || null,
    isEnabled: form.isEnabled,
    triggerType: form.triggerType,
    conditionJson: JSON.stringify({
      field: form.conditionField,
      operator: form.conditionOperator,
      value: numeric,
    }),
    actionType: form.actionType,
    actionParametersJson: JSON.stringify({
      ...(form.actionMessage ? { message: form.actionMessage } : {}),
      ...(form.actionDescription ? { description: form.actionDescription } : {}),
      ...(form.actionPriority ? { priority: Number(form.actionPriority) } : {}),
    }),
  };
}

export function AutomationPage() {
  const [rules, setRules] = useState<AutomationRule[]>([]);
  const [executions, setExecutions] = useState<AutomationRuleExecution[]>([]);
  const [selected, setSelected] = useState<AutomationRule | null>(null);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [modal, setModal] = useState(false);
  const [executionModal, setExecutionModal] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setRules(await apiFetch<AutomationRule[]>("/v1/automation-rules?skip=0&take=100"));
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Otomasyon kuralları yüklenemedi.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const data = await apiFetch<AutomationRule[]>("/v1/automation-rules?skip=0&take=100");
        if (active) setRules(data);
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Otomasyon kuralları yüklenemedi.");
      } finally {
        if (active) setLoading(false);
      }
    })();
    return () => {
      active = false;
    };
  }, []);

  const openCreate = () => {
    setSelected(null);
    setForm(emptyForm);
    setModal(true);
    setError("");
  };

  const openEdit = (rule: AutomationRule) => {
    setSelected(rule);
    setForm(parseForm(rule));
    setModal(true);
    setError("");
  };

  const save = async (event: React.FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setError("");
    try {
      const payload = toPayload(form);
      if (selected) {
        await apiFetch(`/v1/automation-rules/${selected.id}`, {
          method: "PATCH",
          body: JSON.stringify({ id: selected.id, ...payload }),
        });
        setNotice("Otomasyon kuralı güncellendi.");
      } else {
        await apiFetch("/v1/automation-rules", { method: "POST", body: JSON.stringify(payload) });
        setNotice("Otomasyon kuralı oluşturuldu.");
      }
      setModal(false);
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Kural kaydedilemedi.");
    } finally {
      setSaving(false);
    }
  };

  const toggle = async (rule: AutomationRule) => {
    try {
      await apiFetch(`/v1/automation-rules/${rule.id}`, {
        method: "PATCH",
        body: JSON.stringify({
          id: rule.id,
          name: rule.name,
          description: rule.description,
          isEnabled: !rule.isEnabled,
          triggerType: rule.triggerType,
          conditionJson: rule.conditionJson,
          actionType: rule.actionType,
          actionParametersJson: rule.actionParametersJson,
        }),
      });
      setNotice(rule.isEnabled ? "Kural devre dışı bırakıldı." : "Kural etkinleştirildi.");
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Kural durumu değiştirilemedi.");
    }
  };

  const showExecutions = async (rule: AutomationRule) => {
    setSelected(rule);
    setExecutionModal(true);
    setError("");
    try {
      setExecutions(await apiFetch<AutomationRuleExecution[]>(`/v1/automation-rules/${rule.id}/executions?skip=0&take=50`));
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Çalıştırma geçmişi yüklenemedi.");
    }
  };

  const setValue = (key: keyof FormState, value: string | boolean) =>
    setForm((current) => ({ ...current, [key]: value }));

  return (
    <>
      <PageHeader
        eyebrow="Otomasyon stüdyosu"
        title="Otomasyon"
        description="İş olaylarına koşul ve aksiyon bağlayarak tekrar eden operasyonları otomatikleştirin."
        action={<button className="primary-button" onClick={openCreate}>+ Yeni kural</button>}
      />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <div className="card overflow-hidden">
        {loading ? <div className="flex justify-center py-16 text-teal"><Spinner /></div> : rules.length === 0 ? <EmptyState label="Henüz otomasyon kuralı bulunmuyor." /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="bg-slate-50 text-xs uppercase tracking-wider text-slate-500">
                <tr><th className="px-5 py-3">Kural</th><th className="px-5 py-3">Tetikleyici</th><th className="px-5 py-3">Aksiyon</th><th className="px-5 py-3">Durum</th><th className="px-5 py-3" /></tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {rules.map((rule) => (
                  <tr key={rule.id}>
                    <td className="px-5 py-4"><p className="font-bold text-ink">{rule.name}</p><p className="mt-1 text-xs text-slate-400">{rule.description || "Açıklama yok"}</p></td>
                    <td className="px-5 py-4 text-slate-600">{triggers.find(([value]) => value === rule.triggerType)?.[1] ?? rule.triggerType}</td>
                    <td className="px-5 py-4 text-slate-600">{actions.find(([value]) => value === rule.actionType)?.[1] ?? rule.actionType}</td>
                    <td className="px-5 py-4"><StatusPill value={rule.isEnabled} /></td>
                    <td className="whitespace-nowrap px-5 py-4 text-right">
                      <button className="mr-3 text-xs font-bold text-teal hover:underline" onClick={() => void showExecutions(rule)}>Geçmiş</button>
                      <button className="mr-3 text-xs font-bold text-teal hover:underline" onClick={() => openEdit(rule)}>Düzenle</button>
                      <button className="text-xs font-bold text-slate-500 hover:underline" onClick={() => void toggle(rule)}>{rule.isEnabled ? "Devre dışı" : "Etkinleştir"}</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
      {modal && <Modal title={selected ? "Otomasyon kuralını düzenle" : "Yeni otomasyon kuralı"} onClose={() => setModal(false)}>
        <form className="space-y-4" onSubmit={save}>
          <label className="block text-sm font-semibold text-ink">Kural adı<input className="field mt-2" required value={form.name} onChange={(event) => setValue("name", event.target.value)} /></label>
          <label className="block text-sm font-semibold text-ink">Açıklama<textarea className="field mt-2" value={form.description} onChange={(event) => setValue("description", event.target.value)} /></label>
          <div className="grid gap-4 sm:grid-cols-2">
            <label className="block text-sm font-semibold text-ink">Tetikleyici<select className="field mt-2" value={form.triggerType} onChange={(event) => setValue("triggerType", event.target.value)}>{triggers.map(([value, label]) => <option key={value} value={value}>{label}</option>)}</select></label>
            <label className="block text-sm font-semibold text-ink">Aksiyon<select className="field mt-2" value={form.actionType} onChange={(event) => setValue("actionType", event.target.value)}>{actions.map(([value, label]) => <option key={value} value={value}>{label}</option>)}</select></label>
          </div>
          <div className="rounded-xl bg-slate-50 p-4">
            <p className="mb-3 text-xs font-bold uppercase tracking-wider text-slate-500">Koşul</p>
            <div className="grid gap-3 sm:grid-cols-3">
              <input className="field" placeholder="Alan (status)" value={form.conditionField} onChange={(event) => setValue("conditionField", event.target.value)} />
              <select className="field" value={form.conditionOperator} onChange={(event) => setValue("conditionOperator", event.target.value)}><option value="equals">eşittir</option><option value="not-equals">eşit değildir</option><option value="greater">büyüktür</option><option value="less">küçüktür</option></select>
              <input className="field" placeholder="Değer" value={form.conditionValue} onChange={(event) => setValue("conditionValue", event.target.value)} />
            </div>
          </div>
          <label className="block text-sm font-semibold text-ink">Bildirim metni<input className="field mt-2" value={form.actionMessage} onChange={(event) => setValue("actionMessage", event.target.value)} placeholder="Yeni iş emri oluşturuldu." /></label>
          <label className="flex items-center gap-2 text-sm font-semibold text-ink"><input type="checkbox" checked={form.isEnabled} onChange={(event) => setValue("isEnabled", event.target.checked)} /> Kural etkin</label>
          <div className="flex justify-end gap-3"><button type="button" className="secondary-button" onClick={() => setModal(false)}>Vazgeç</button><button className="primary-button" disabled={saving}>{saving && <Spinner />} Kaydet</button></div>
        </form>
      </Modal>}
      {executionModal && <Modal title={`${selected?.name ?? "Kural"} · çalıştırma geçmişi`} onClose={() => setExecutionModal(false)}>
        {executions.length === 0 ? <EmptyState label="Bu kural henüz çalıştırılmamış." /> : <div className="space-y-3">{executions.map((execution) => <div key={execution.id} className="rounded-xl border border-slate-100 p-4"><div className="flex items-center justify-between gap-3"><StatusPill value={execution.success} /><span className="text-xs text-slate-400">{new Date(execution.executedAt).toLocaleString("tr-TR")}</span></div><p className="mt-2 text-sm text-slate-600">{execution.detail}</p></div>)}</div>}
      </Modal>}
    </>
  );
}
