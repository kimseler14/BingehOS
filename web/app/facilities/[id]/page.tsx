import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default async function FacilityDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <EntityDetailPage title="Tesis detayı" eyebrow="Tesis yönetimi" endpoint={`/v1/facilities/${id}`} backHref="/facilities" labels={{ name: "Tesis adı", code: "Kod", address: "Adres", timeZone: "Zaman dilimi", parentFacilityId: "Üst tesis" }} />;
}
