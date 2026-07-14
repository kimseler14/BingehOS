import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default function PartDetailRoute({ params }: { params: { id: string } }) {
  return <EntityDetailPage title="Parça detayı" eyebrow="Envanter" endpoint={`/v1/parts/${params.id}`} backHref="/inventory" labels={{ partNumber: "Parça numarası", name: "Parça adı", description: "Açıklama", unitOfMeasure: "Birim", isActive: "Durum" }} />;
}
