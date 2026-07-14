import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default function WorkOrderDetailRoute({ params }: { params: { id: string } }) {
  return <EntityDetailPage title="İş emri detayı" eyebrow="Bakım operasyonu" endpoint={`/v1/work-orders/${params.id}`} backHref="/work-orders" labels={{ description: "Açıklama", assetId: "Varlık ID", status: "Durum" }} />;
}
