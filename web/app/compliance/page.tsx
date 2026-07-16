"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { EntityListPage, EntityConfig } from "../../components/EntityListPage";
import { EntityDetailPage } from "../../components/EntityDetailPage";
import { Spinner } from "../../components/Ui";

const complianceConfig: EntityConfig = {
  title: "Uygunluk Kayıtları",
  eyebrow: "Uygunluk yönetimi",
  description: "Mevzuat uygunluk kayıtlarını ve KVKK onaylarını yönetin.",
  endpoint: "/v1/compliance-records",
  basePath: "/compliance",
  singular: "kayıt",
  columns: [
    { key: "title", label: "Başlık" },
    { key: "status", label: "Durum", type: "status" },
    { key: "dueDate", label: "Bitiş", type: "date" },
  ],
  createFields: [
    { name: "title", label: "Başlık", required: true },
    { name: "status", label: "Durum" },
  ],
  editFields: [
    { name: "title", label: "Başlık", required: true },
    { name: "status", label: "Durum" },
  ],
  detail: {
    title: "Uygunluk kaydı detayı",
    eyebrow: "Uygunluk yönetimi",
    endpoint: (id: string) => `/v1/compliance-records/${id}`,
    backHref: "/compliance",
    labels: { title: "Başlık", status: "Durum", dueDate: "Bitiş" },
  },
};

export default function CompliancePage() {
  return <EntityListPage config={complianceConfig} />;
}
