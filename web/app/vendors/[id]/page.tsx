import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default async function VendorDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <EntityDetailPage title="Tedarikçi detayı" eyebrow="Tedarikçi yönetimi" endpoint={`/v1/vendors/${id}`} backHref="/vendors" labels={{ name: "Firma", taxNumber: "Vergi numarası", contactEmail: "E-posta", phone: "Telefon", isActive: "Durum" }} />;
}
