import { EntityListPage, EntityConfig } from "../../../components/EntityListPage";

const config: EntityConfig = {
  title: "Roller",
  eyebrow: "Yönetim · Identity",
  description: "Rol tanımlayın, açıklamalarını düzenleyin ve izin atamalarını detay ekranından yönetin.",
  endpoint: "/v1/roles",
  basePath: "/admin/roles",
  singular: "rol",
  deletable: true,
  columns: [
    { key: "name", label: "Rol" },
    { key: "description", label: "Açıklama" },
    { key: "permissions", label: "İzinler" },
    { key: "isSystem", label: "Sistem", type: "status" },
  ],
  createFields: [
    { name: "name", label: "Rol adı", required: true },
    { name: "description", label: "Açıklama", type: "textarea" },
    { name: "isSystem", label: "Sistem rolü", type: "checkbox" },
  ],
  editFields: [
    { name: "name", label: "Rol adı", required: true },
    { name: "description", label: "Açıklama", type: "textarea" },
  ],
};

export default function RolesPage() {
  return <EntityListPage config={config} />;
}
