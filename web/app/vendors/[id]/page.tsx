import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default function VendorDetailRoute({ params }: { params: { id: string } }) {
  return <EntityDetailPage title="Tedarikçi detayı" eyebrow="Tedarikçi yönetimi" endpoint={`/v1/vendors/${params.id}`} backHref="/vendors" labels={{ name: "Firma", taxNumber: "Vergi numarası", contactEmail: "E-posta", phone: "Telefon", isActive: "Durum" }} />;
}
