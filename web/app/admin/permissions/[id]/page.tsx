import { EntityDetailPage } from "../../../../components/EntityDetailPage";

export default function PermissionDetailRoute({ params }: { params: { id: string } }) {
  return <EntityDetailPage title="İzin detayı" eyebrow="Yönetim · Identity" endpoint={`/v1/permissions/${params.id}`} backHref="/admin/permissions" labels={{ name: "İzin anahtarı", description: "Açıklama" }} />;
}
