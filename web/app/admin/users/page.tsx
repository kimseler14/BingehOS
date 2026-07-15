"use client";

import { EntityListPage, EntityConfig } from "../../../components/EntityListPage";

const config: EntityConfig = {
  title: "Kullanıcılar",
  eyebrow: "Yönetim · Identity",
  description: "Tenant kullanıcılarını ve rollerini yönetin. Bu ekran admin.access izni gerektirir.",
  endpoint: "/v1/users",
  basePath: "/admin/users",
  singular: "kullanıcı",
  deletable: true,
  canCreate: false,
  columns: [
    { key: "fullName", label: "Ad soyad" },
    { key: "email", label: "E-posta" },
    { key: "roles", label: "Roller" },
    { key: "isActive", label: "Durum", type: "status" },
  ],
  createFields: [
    { name: "email", label: "E-posta", required: true },
    { name: "password", label: "Geçici şifre", required: true },
    { name: "fullName", label: "Ad soyad", required: true },
  ],
  editFields: [
    { name: "fullName", label: "Ad soyad", required: true },
    { name: "isActive", label: "Aktif", type: "checkbox", defaultValue: true },
  ],
  detail: {
    title: "Kullanıcı detayı",
    eyebrow: "Yönetim · Identity",
    endpoint: (id: string) => `/v1/users/${id}`,
    backHref: "/admin/users",
    labels: { fullName: "Ad soyad", email: "E-posta", roles: "Roller", isActive: "Durum" },
  },
};

export default function UsersPage() {
  return <EntityListPage config={config} />;
}
