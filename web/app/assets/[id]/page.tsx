import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default function AssetDetailRoute({ params }: { params: { id: string } }) {
  return <EntityDetailPage title="Varlık detayı" eyebrow="Varlık yönetimi" endpoint={`/v1/assets/${params.id}`} backHref="/assets" labels={{ name: "Varlık adı", serialNumber: "Seri numarası", locationCode: "Konum kodu", criticality: "Kritiklik" }} />;
}
