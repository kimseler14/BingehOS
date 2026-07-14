import { EntityListPage, EntityConfig } from "../../components/EntityListPage";

const config: EntityConfig = {
  title: "Tedarikçiler",
  eyebrow: "Tedarikçi yönetimi",
  description: "Tedarikçi iletişim ve vergi bilgilerini, aktiflik durumlarıyla birlikte izleyin.",
  endpoint: "/v1/vendors",
  basePath: "/vendors",
  singular: "tedarikçi",
  activeFilter: true,
  columns: [
    { key: "name", label: "Firma" },
    { key: "taxNumber", label: "Vergi no" },
    { key: "contactEmail", label: "E-posta" },
    { key: "phone", label: "Telefon" },
    { key: "isActive", label: "Durum", type: "status" },
  ],
  createFields: [
    { name: "name", label: "Firma adı", required: true },
    { name: "taxNumber", label: "Vergi numarası" },
    { name: "contactEmail", label: "E-posta", type: "text" },
    { name: "phone", label: "Telefon" },
    { name: "isActive", label: "Aktif", type: "checkbox", defaultValue: true },
  ],
  editFields: [
    { name: "name", label: "Firma adı", required: true },
    { name: "contactEmail", label: "E-posta" },
    { name: "phone", label: "Telefon" },
    { name: "isActive", label: "Aktif", type: "checkbox" },
  ],
};

export default function VendorsPage() {
  return <EntityListPage config={config} />;
}
