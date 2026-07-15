import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default async function WorkOrderDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <EntityDetailPage title="İş emri detayı" eyebrow="Bakım operasyonu" endpoint={`/v1/work-orders/${id}`} backHref="/work-orders" labels={{ description: "Açıklama", assetId: "Varlık ID", status: "Durum" }} />;
}
