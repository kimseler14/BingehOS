import "./globals.css";
import { AuthProvider } from "../components/AuthProvider";
import { AppShell } from "../components/AppShell";
import { ServiceWorkerRegistration } from "../components/ServiceWorkerRegistration";

export const metadata = {
  title: "BingehOS Operasyon Merkezi",
  description: "BingehOS CMMS web arayüzü",
  manifest: "/manifest.webmanifest",
};

export const viewport = {
  themeColor: "#0f172a",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="tr">
      <body>
        <AuthProvider>
          <ServiceWorkerRegistration />
          <AppShell>{children}</AppShell>
        </AuthProvider>
      </body>
    </html>
  );
}
