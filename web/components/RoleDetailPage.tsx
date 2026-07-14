"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { apiFetch } from "../lib/api";
import type { Permission, Role } from "../lib/types";
import { ErrorNotice, PageHeader, Spinner } from "./Ui";

export function RoleDetailPage({ id }: { id: string }) {
  const [role, setRole] = useState<Role | null>(null);
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [selectedPermission, setSelectedPermission] = useState("");
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");

  const load = useCallback(async () => {
    try {
      const [roleData, permissionData] = await Promise.all([apiFetch<Role>(`/v1/roles/${id}`), apiFetch<Permission[]>("/v1/permissions?skip=0&take=100")]);
      setRole(roleData);
      setPermissions(permissionData);
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Rol detayı yüklenemedi.");
    }
  }, [id]);

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const [role, permissions] = await Promise.all([
          apiFetch<Role>(`/v1/roles/${id}`),
          apiFetch<Permission[]>("/v1/permissions"),
        ]);
        if (active) {
          setRole(role);
          setPermissions(permissions);
        }
      } catch (cause) {
        if (active) setError(cause instanceof Error ? cause.message : "Rol detayları yüklenemedi.");
      }
    })();
    return () => { active = false; };
  }, [id]);

  async function changePermission(permissionId: string, remove = false) {
    try {
      await apiFetch(`/v1/roles/${id}/permissions/${permissionId}`, { method: remove ? "DELETE" : "POST" });
      setNotice(remove ? "İzin kaldırıldı." : "İzin atandı.");
      await load();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "İzin işlemi başarısız.");
    }
  }

  if (error && !role) return <ErrorNotice message={error} />;
  if (!role) return <div className="flex justify-center py-20 text-teal"><Spinner /></div>;
  const assigned = new Set(role.permissions);
  const available = permissions.filter((permission) => !assigned.has(permission.name));

  return (
    <>
      <PageHeader eyebrow="Yönetim · Rol" title={role.name} description={role.description || "Rol izinlerini yönetin."} action={<Link href="/admin/roles" className="secondary-button">← Rollere dön</Link>} />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <div className="grid gap-6 xl:grid-cols-2">
        <section className="card p-6"><h2 className="font-black text-ink">Atanmış izinler</h2><div className="mt-5 space-y-2">{role.permissions.length === 0 ? <p className="text-sm text-slate-400">Bu role henüz izin atanmadı.</p> : role.permissions.map((permission) => { const item = permissions.find((candidate) => candidate.name === permission); return <div key={permission} className="flex items-center justify-between rounded-xl bg-slate-50 px-4 py-3"><span className="text-sm font-semibold text-ink">{permission}</span>{item && <button className="text-xs font-bold text-rose-600" onClick={() => void changePermission(item.id, true)}>Kaldır</button>}</div>; })}</div></section>
        <section className="card p-6"><h2 className="font-black text-ink">İzin ata</h2><div className="mt-5 space-y-4"><select className="field" value={selectedPermission} onChange={(event) => setSelectedPermission(event.target.value)}><option value="">İzin seçin</option>{available.map((permission) => <option key={permission.id} value={permission.id}>{permission.name}</option>)}</select><button className="primary-button" disabled={!selectedPermission} onClick={() => { void changePermission(selectedPermission); setSelectedPermission(""); }}>Role izin ata</button></div></section>
      </div>
    </>
  );
}
