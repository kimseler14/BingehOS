import "./globals.css";
import { AuthProvider } from "../components/AuthProvider";
import { AppShell } from "../components/AppShell";

export const metadata = {
  title: "BingehOS Operasyon Merkezi",
  description: "BingehOS CMMS web arayüzü",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="tr">
      <body>
        <AuthProvider>
          <AppShell>{children}</AppShell>
        </AuthProvider>
      </body>
    </html>
  );
}
