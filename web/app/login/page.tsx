"use client";

import { FormEvent, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { apiFetch, ApiError } from "../../lib/api";
import type { LoginResponse } from "../../lib/types";
import { useAuth } from "../../components/AuthProvider";
import { ErrorNotice, Spinner } from "../../components/Ui";

export default function LoginPage() {
  const router = useRouter();
  const { user, ready, setSession } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (ready && user) router.replace("/dashboard");
  }, [ready, router, user]);

  if (ready && user) return null;

  async function submit(event: FormEvent) {
    event.preventDefault();
    setLoading(true);
    setError("");
    try {
      const session = await apiFetch<LoginResponse>("/v1/auth/login", { method: "POST", body: JSON.stringify({ email, password }) });
      setSession(session);
      router.replace("/dashboard");
    } catch (cause) {
      setError(cause instanceof ApiError ? cause.message : "Giriş yapılamadı.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="flex min-h-screen items-center justify-center bg-ink px-4 py-12">
      <div className="grid w-full max-w-5xl overflow-hidden rounded-3xl bg-white shadow-2xl lg:grid-cols-[1.1fr_0.9fr]">
        <div className="hidden bg-gradient-to-br from-ink via-slate-800 to-teal p-12 text-white lg:block">
          <p className="text-sm font-bold uppercase tracking-[0.25em] text-teal">BingehOS CMMS</p>
          <h1 className="mt-20 text-5xl font-black leading-tight">Bakım operasyonunu tek merkezden yönetin.</h1>
          <p className="mt-6 max-w-md text-slate-300">Varlıklar, iş emirleri, envanter ve ekipler için sade, çok kiracılı operasyon ekranı.</p>
          <div className="mt-16 grid grid-cols-3 gap-3 text-center text-xs text-slate-300">
            <div className="rounded-2xl bg-white/10 p-4"><b className="block text-2xl text-white">360°</b> görünürlük</div>
            <div className="rounded-2xl bg-white/10 p-4"><b className="block text-2xl text-white">RLS</b> izolasyon</div>
            <div className="rounded-2xl bg-white/10 p-4"><b className="block text-2xl text-white">24/7</b> hazır</div>
          </div>
        </div>
        <div className="p-8 sm:p-12">
          <div className="mb-10">
            <p className="text-2xl font-black tracking-tight text-ink">Binge<span className="text-teal">hOS</span></p>
            <h2 className="mt-10 text-3xl font-black text-ink">Tekrar hoş geldiniz</h2>
            <p className="mt-2 text-sm text-slate-500">Operasyon merkezine erişmek için giriş yapın.</p>
          </div>
          {error && <div className="mb-5"><ErrorNotice message={error} /></div>}
          <form className="space-y-5" onSubmit={submit}>
            <label className="block text-sm font-semibold text-ink">E-posta<input className="field mt-2" type="email" required value={email} onChange={(event) => setEmail(event.target.value)} placeholder="admin@firma.com" /></label>
            <label className="block text-sm font-semibold text-ink">Şifre<input className="field mt-2" type="password" required value={password} onChange={(event) => setPassword(event.target.value)} placeholder="••••••••" /></label>
            <button className="primary-button w-full py-3" disabled={loading}>{loading && <Spinner />} Giriş yap</button>
          </form>
          <p className="mt-8 text-center text-xs text-slate-400">API adresi: Next.js proxy üzerinden yapılandırılır.</p>
        </div>
      </div>
    </main>
  );
}
