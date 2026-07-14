import { EntityDetailPage } from "../../../../components/EntityDetailPage";

export default async function PermissionDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <EntityDetailPage title="İzin detayı" eyebrow="Yönetim · Identity" endpoint={`/v1/permissions/${id}`} backHref="/admin/permissions" labels={{ name: "İzin anahtarı", description: "Açıklama" }} />;
}
