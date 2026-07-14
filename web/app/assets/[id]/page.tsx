import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default async function AssetDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <EntityDetailPage title="Varlık detayı" eyebrow="Varlık yönetimi" endpoint={`/v1/assets/${id}`} backHref="/assets" labels={{ name: "Varlık adı", serialNumber: "Seri numarası", locationCode: "Konum kodu", criticality: "Kritiklik" }} />;
}
