"use client";

import { EntityListPage, EntityConfig } from "../../../components/EntityListPage";

const config: EntityConfig = {
  title: "Maliyet Merkezleri",
  eyebrow: "Finans yönetimi",
  description: "Bütçe takibi için maliyet merkezlerini yönetin.",
  endpoint: "/v1/cost-centers",
  basePath: "/finance/cost-centers",
  singular: "maliyet merkezi",
  columns: [
    { key: "code", label: "Kod" },
    { key: "name", label: "Ad" },
    { key: "budgetMinor", label: "Bütçe" },
    { key: "currency", label: "Para birimi" },
    { key: "isActive", label: "Durum", type: "status" },
  ],
  createFields: [
    { name: "code", label: "Kod", required: true },
    { name: "name", label: "Ad", required: true },
    { name: "budgetMinor", label: "Bütçe (kuruş)", type: "number" },
    { name: "currency", label: "Para birimi", placeholder: "TRY" },
    { name: "isActive", label: "Aktif", type: "checkbox", defaultValue: true },
  ],
  editFields: [
    { name: "code", label: "Kod", required: true },
    { name: "name", label: "Ad", required: true },
    { name: "budgetMinor", label: "Bütçe (kuruş)", type: "number" },
    { name: "currency", label: "Para birimi" },
    { name: "isActive", label: "Aktif", type: "checkbox" },
  ],
  detail: {
    title: "Maliyet merkezi detayı",
    eyebrow: "Finans yönetimi",
    endpoint: (id: string) => `/v1/cost-centers/${id}`,
    backHref: "/finance/cost-centers",
    labels: { code: "Kod", name: "Ad", budgetMinor: "Bütçe", currency: "Para birimi", isActive: "Durum" },
  },
};

export default function CostCentersPage() {
  return <EntityListPage config={config} />;
}
