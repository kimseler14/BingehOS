import { EntityListPage, EntityConfig } from "../../../components/EntityListPage";

const config: EntityConfig = {
  title: "İzinler",
  eyebrow: "Yönetim · Identity",
  description: "Sistemdeki yetki anahtarlarını görüntüleyin ve yeni izin tanımları ekleyin.",
  endpoint: "/v1/permissions",
  basePath: "/admin/permissions",
  singular: "izin",
  columns: [
    { key: "name", label: "İzin anahtarı" },
    { key: "description", label: "Açıklama" },
  ],
  createFields: [
    { name: "name", label: "İzin anahtarı", required: true, placeholder: "inventory.read" },
    { name: "description", label: "Açıklama", type: "textarea" },
  ],
};

export default function PermissionsPage() {
  return <EntityListPage config={config} />;
}
