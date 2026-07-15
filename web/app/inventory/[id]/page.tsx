import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default async function PartDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <EntityDetailPage title="Parça detayı" eyebrow="Envanter" endpoint={`/v1/parts/${id}`} backHref="/inventory" labels={{ partNumber: "Parça numarası", name: "Parça adı", description: "Açıklama", unitOfMeasure: "Birim", isActive: "Durum" }} />;
}
