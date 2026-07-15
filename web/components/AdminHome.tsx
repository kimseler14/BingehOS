"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { apiFetch } from "../lib/api";
import type { Permission, Role } from "../lib/types";
import { ErrorNotice, PageHeader, Spinner } from "./Ui";

export function AdminHome() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [register, setRegister] = useState({ email: "", password: "", fullName: "" });
  const [assignment, setAssignment] = useState({ userId: "", roleId: "" });
  const [notice, setNotice] = useState("");
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    void apiFetch<Role[]>("/v1/roles?skip=0&take=100").then(setRoles).catch((cause) => setError(cause instanceof Error ? cause.message : "Roller yüklenemedi."));
    void apiFetch<Permission[]>("/v1/permissions?skip=0&take=100").then(setPermissions).catch((cause) => setError(cause instanceof Error ? cause.message : "İzinler yüklenemedi."));
  }, []);

  async function registerUser(event: React.FormEvent) {
    event.preventDefault();
    setSaving(true);
    try {
      await apiFetch("/v1/auth/register", { method: "POST", body: JSON.stringify(register) });
      setNotice("Kullanıcı kaydı oluşturuldu.");
      setRegister({ email: "", password: "", fullName: "" });
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Kullanıcı oluşturulamadı.");
    } finally {
      setSaving(false);
    }
  }

  async function assignRole(event: React.FormEvent) {
    event.preventDefault();
    setSaving(true);
    try {
      await apiFetch("/v1/auth/assign-role", { method: "POST", body: JSON.stringify(assignment) });
      setNotice("Rol kullanıcıya atandı.");
      setAssignment({ userId: "", roleId: "" });
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : "Rol atanamadı.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <>
      <PageHeader eyebrow="Yönetim merkezi" title="Identity yönetimi" description="Kullanıcı, rol ve izin operasyonlarını tek ekrandan yönetin." />
      {notice && <button className="mb-4 w-full rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-left text-sm text-emerald-700" onClick={() => setNotice("")}>{notice} ×</button>}
      {error && <div className="mb-4"><ErrorNotice message={error} /></div>}
      <div className="mb-6 grid gap-4 sm:grid-cols-3"><Link href="/admin/users" className="card p-5 hover:border-teal"><p className="text-xs font-bold uppercase tracking-wider text-teal">Kullanıcılar</p><p className="mt-4 text-sm text-slate-500">Listele, düzenle ve pasifleştir.</p></Link><Link href="/admin/roles" className="card p-5 hover:border-teal"><p className="text-xs font-bold uppercase tracking-wider text-teal">Roller</p><p className="mt-4 text-sm text-slate-500">CRUD ve izin atama işlemleri.</p></Link><Link href="/admin/permissions" className="card p-5 hover:border-teal"><p className="text-xs font-bold uppercase tracking-wider text-teal">İzinler</p><p className="mt-4 text-sm text-slate-500">Yetki anahtarlarını yönetin.</p></Link></div>
      <div className="grid gap-6 xl:grid-cols-2">
        <section className="card p-6"><h2 className="text-xl font-black text-ink">Yeni kullanıcı kaydı</h2><p className="mt-1 text-sm text-slate-500">Kullanıcı varsayılan User rolüyle oluşturulur.</p><form className="mt-5 space-y-4" onSubmit={registerUser}><input className="field" required type="text" placeholder="Ad soyad" value={register.fullName} onChange={(event) => setRegister({ ...register, fullName: event.target.value })} /><input className="field" required type="email" placeholder="E-posta" value={register.email} onChange={(event) => setRegister({ ...register, email: event.target.value })} /><input className="field" required type="password" placeholder="Geçici şifre" value={register.password} onChange={(event) => setRegister({ ...register, password: event.target.value })} /><button className="primary-button" disabled={saving}>{saving && <Spinner />} Kullanıcı oluştur</button></form></section>
        <section className="card p-6"><h2 className="text-xl font-black text-ink">Rol ata</h2><p className="mt-1 text-sm text-slate-500">Kullanıcı ve rol UUID değerlerini girin.</p><form className="mt-5 space-y-4" onSubmit={assignRole}><input className="field" required placeholder="Kullanıcı ID" value={assignment.userId} onChange={(event) => setAssignment({ ...assignment, userId: event.target.value })} /><select className="field" required value={assignment.roleId} onChange={(event) => setAssignment({ ...assignment, roleId: event.target.value })}><option value="">Rol seçin</option>{roles.map((role) => <option key={role.id} value={role.id}>{role.name}</option>)}</select><button className="primary-button" disabled={saving}>{saving && <Spinner />} Rol ata</button></form><div className="mt-6 border-t border-slate-100 pt-5"><p className="text-xs font-bold uppercase tracking-wider text-slate-400">Mevcut izinler</p><div className="mt-3 flex flex-wrap gap-2">{permissions.slice(0, 8).map((permission) => <span key={permission.id} className="rounded-full bg-slate-100 px-2.5 py-1 text-xs text-slate-600">{permission.name}</span>)}</div></div></section>
      </div>
    </>
  );
}
