"use client";

export default function OfflinePage() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-mist px-6">
      <section className="max-w-md rounded-3xl border border-slate-200 bg-white p-8 text-center shadow-xl">
        <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-2xl bg-ink text-2xl font-black text-teal">B</div>
        <p className="mt-6 text-xs font-bold uppercase tracking-[0.2em] text-teal">BingehOS</p>
        <h1 className="mt-2 text-2xl font-black text-ink">Çevrimdışısınız</h1>
        <p className="mt-3 text-sm leading-6 text-slate-500">İnternet bağlantısı kurulamadı. Bağlantı geri geldiğinde sayfayı yenileyerek operasyon ekranına devam edebilirsiniz.</p>
        <button className="primary-button mt-6" onClick={() => window.location.reload()}>Tekrar dene</button>
      </section>
    </main>
  );
}
