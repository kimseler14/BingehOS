"use client";

import { EntityListPage, EntityConfig } from "../../components/EntityListPage";

const config: EntityConfig = {
  title: "Personel",
  eyebrow: "Ekip yönetimi",
  description: "Bakım ekibinizi uzmanlık, departman ve iletişim bilgileriyle yönetin.",
  endpoint: "/v1/workers",
  basePath: "/workers",
  singular: "personel",
  activeFilter: true,
  columns: [
    { key: "firstName", label: "Ad" },
    { key: "lastName", label: "Soyad" },
    { key: "employeeNumber", label: "Sicil no" },
    { key: "trade", label: "Uzmanlık" },
    { key: "department", label: "Departman" },
    { key: "isActive", label: "Durum", type: "status" },
  ],
  createFields: [
    { name: "firstName", label: "Ad", required: true },
    { name: "lastName", label: "Soyad", required: true },
    { name: "employeeNumber", label: "Sicil numarası" },
    { name: "trade", label: "Uzmanlık / zanaat" },
    { name: "department", label: "Departman" },
    { name: "phone", label: "Telefon" },
    { name: "isActive", label: "Aktif", type: "checkbox", defaultValue: true },
  ],
  detail: {
    title: "Personel detayı",
    eyebrow: "Ekip yönetimi",
    endpoint: (id: string) => `/v1/workers/${id}`,
    backHref: "/workers",
    labels: { firstName: "Ad", lastName: "Soyad", employeeNumber: "Sicil numarası", trade: "Uzmanlık", department: "Departman", phone: "Telefon", isActive: "Durum" },
  },
};

export default function WorkersPage() {
  return <EntityListPage config={config} />;
}
