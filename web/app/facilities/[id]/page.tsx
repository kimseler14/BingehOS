import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default function FacilityDetailRoute({ params }: { params: { id: string } }) {
  return <EntityDetailPage title="Tesis detayı" eyebrow="Tesis yönetimi" endpoint={`/v1/facilities/${params.id}`} backHref="/facilities" labels={{ name: "Tesis adı", code: "Kod", address: "Adres", timeZone: "Zaman dilimi", parentFacilityId: "Üst tesis" }} />;
}
