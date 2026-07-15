"use client";

import { EntityListPage, EntityConfig } from "../../components/EntityListPage";

const config: EntityConfig = {
  title: "Tesisler",
  eyebrow: "Tesis yönetimi",
  description: "Tesis ağacınızı adres, zaman dilimi ve üst tesis ilişkileriyle yönetin.",
  endpoint: "/v1/facilities",
  basePath: "/facilities",
  singular: "tesis",
  columns: [
    { key: "name", label: "Tesis" },
    { key: "code", label: "Kod" },
    { key: "address", label: "Adres" },
    { key: "timeZone", label: "Zaman dilimi" },
  ],
  createFields: [
    { name: "name", label: "Tesis adı", required: true, placeholder: "Merkez fabrika" },
    { name: "code", label: "Tesis kodu" },
    { name: "address", label: "Adres" },
    { name: "timeZone", label: "Zaman dilimi", placeholder: "Europe/Istanbul" },
    { name: "parentFacilityId", label: "Üst tesis ID" },
  ],
  editFields: [
    { name: "name", label: "Tesis adı", required: true },
    { name: "address", label: "Adres" },
    { name: "timeZone", label: "Zaman dilimi" },
    { name: "parentFacilityId", label: "Üst tesis ID" },
  ],
  detail: {
    title: "Tesis detayı",
    eyebrow: "Tesis yönetimi",
    endpoint: (id: string) => `/v1/facilities/${id}`,
    backHref: "/facilities",
    labels: { name: "Tesis adı", code: "Kod", address: "Adres", timeZone: "Zaman dilimi", parentFacilityId: "Üst tesis" },
  },
};

export default function FacilitiesPage() {
  return <EntityListPage config={config} />;
}
