"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { EntityListPage, EntityConfig } from "../../../components/EntityListPage";
import { RoleDetailPage } from "../../../components/RoleDetailPage";
import { Spinner } from "../../../components/Ui";

const config: EntityConfig = {
  title: "Roller",
  eyebrow: "Yönetim · Identity",
  description: "Rol tanımlayın, açıklamalarını düzenleyin ve izin atamalarını detay ekranından yönetin.",
  endpoint: "/v1/roles",
  basePath: "/admin/roles",
  singular: "rol",
  deletable: true,
  canDelete: (item) => item.isSystem !== true,
  columns: [
    { key: "name", label: "Rol" },
    { key: "description", label: "Açıklama" },
    { key: "permissions", label: "İzinler" },
    { key: "isSystem", label: "Sistem", type: "status" },
  ],
  createFields: [
    { name: "name", label: "Rol adı", required: true },
    { name: "description", label: "Açıklama", type: "textarea" },
    { name: "isSystem", label: "Sistem rolü", type: "checkbox", defaultValue: false },
  ],
  editFields: [
    { name: "name", label: "Rol adı", required: true },
    { name: "description", label: "Açıklama", type: "textarea" },
  ],
};

function RolesContent() {
  const searchParams = useSearchParams();
  const id = searchParams.get("id");
  if (id) return <RoleDetailPage id={id} />;
  return <EntityListPage config={config} />;
}

export default function RolesPage() {
  return (
    <Suspense fallback={<div className="flex justify-center py-20 text-teal"><Spinner /></div>}>
      <RolesContent />
    </Suspense>
  );
}
