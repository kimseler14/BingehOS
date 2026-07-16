"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { EntityListPage, EntityConfig } from "../../components/EntityListPage";
import { EntityDetailPage } from "../../components/EntityDetailPage";
import { Spinner } from "../../components/Ui";

const kvkkConfig: EntityConfig = {
  title: "KVKK Onayları",
  eyebrow: "Kişisel verilerin korunması",
  description: "KVKK kapsamında kullanıcı onaylarını görüntüleyin.",
  endpoint: "/v1/kvkk-consents",
  basePath: "/kvkk",
  singular: "onay",
  canCreate: false,
  columns: [
    { key: "consentType", label: "Onay türü" },
    { key: "version", label: "Versiyon" },
    { key: "grantedAt", label: "Verilme", type: "date" },
    { key: "revokedAt", label: "İptal", type: "date" },
  ],
  createFields: [],
  detail: {
    title: "KVKK onay detayı",
    eyebrow: "Kişisel verilerin korunması",
    endpoint: (id: string) => `/v1/kvkk-consents/${id}`,
    backHref: "/kvkk",
    labels: { consentType: "Onay türü", version: "Versiyon", grantedAt: "Verilme", revokedAt: "İptal", userId: "Kullanıcı" },
  },
};

export default function KvkkPage() {
  return <EntityListPage config={kvkkConfig} />;
}
