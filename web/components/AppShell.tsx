"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "./AuthProvider";

const navigation = [
  { href: "/dashboard", label: "Dashboard", icon: "⌂" },
  { href: "/work-orders", label: "İş Emirleri", icon: "✓" },
  { href: "/assets", label: "Varlıklar", icon: "◈" },
  { href: "/facilities", label: "Tesisler", icon: "⌘" },
  { href: "/inventory", label: "Envanter", icon: "▦" },
  { href: "/automation", label: "Otomasyon", icon: "⚡" },
  { href: "/plugins", label: "Eklentiler", icon: "✦" },
  { href: "/insights", label: "Analiz / Öngörüler", icon: "◉" },
  { href: "/workers", label: "Personel", icon: "♙" },
  { href: "/vendors", label: "Tedarikçiler", icon: "◇" },
  { href: "/admin/users", label: "Yönetim", icon: "⚙" },
];

export function AppShell({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const { user, ready, logout } = useAuth();

  if (pathname === "/login") {
    return <>{children}</>;
  }

  if (!ready) {
    return <div className="flex min-h-screen items-center justify-center text-slate-500">Yükleniyor…</div>;
  }

  if (!user) {
    if (typeof window !== "undefined" && pathname !== "/login") window.location.assign("/login");
    return null;
  }

  return (
    <div className="min-h-screen bg-mist lg:flex">
      <aside className="w-full border-b border-slate-200 bg-ink text-white lg:fixed lg:inset-y-0 lg:w-64 lg:border-b-0">
        <div className="flex items-center justify-between px-6 py-6 lg:block">
          <Link href="/dashboard" className="text-xl font-black tracking-tight">
            Binge<span className="text-teal">hOS</span>
          </Link>
          <span className="rounded-full bg-white/10 px-2 py-1 text-[10px] font-bold uppercase tracking-widest text-teal">
            CMMS
          </span>
        </div>
        <nav className="flex gap-1 overflow-x-auto px-3 pb-4 lg:block lg:space-y-1 lg:px-4">
          {navigation.map((item) => {
            const active = pathname === item.href || pathname.startsWith(`${item.href}/`);
            return (
              <Link
                key={item.href}
                href={item.href}
                className={`flex min-w-max items-center gap-3 rounded-xl px-3 py-2.5 text-sm transition ${
                  active ? "bg-white text-ink shadow-lg" : "text-slate-300 hover:bg-white/10 hover:text-white"
                }`}
              >
                <span className="w-5 text-center text-base">{item.icon}</span>
                {item.label}
              </Link>
            );
          })}
        </nav>
        <div className="hidden border-t border-white/10 p-4 text-xs text-slate-400 lg:absolute lg:inset-x-0 lg:bottom-0 lg:block">
          Çok kiracılı bakım operasyonu
          <div className="mt-2 text-teal">v0.1 • canlıya hazır</div>
        </div>
      </aside>
      <main className="w-full lg:ml-64">
        <header className="flex items-center justify-between border-b border-slate-200 bg-white px-5 py-4 lg:px-8">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.2em] text-teal">Operasyon merkezi</p>
            <p className="mt-1 text-sm text-slate-500">Günün bakım, envanter ve ekip görünümü</p>
          </div>
          <div className="flex items-center gap-3">
            <div className="hidden text-right sm:block">
              <p className="text-sm font-bold text-ink">{user.fullName}</p>
              <p className="text-xs text-slate-500">{user.roles.join(" · ") || user.email}</p>
            </div>
            <button onClick={logout} className="secondary-button">Çıkış</button>
          </div>
        </header>
        <div className="p-5 lg:p-8">{children}</div>
      </main>
    </div>
  );
}
