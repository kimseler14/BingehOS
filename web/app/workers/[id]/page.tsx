import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default async function WorkerDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <EntityDetailPage title="Personel detayı" eyebrow="Ekip yönetimi" endpoint={`/v1/workers/${id}`} backHref="/workers" labels={{ firstName: "Ad", lastName: "Soyad", employeeNumber: "Sicil numarası", trade: "Uzmanlık", department: "Departman", phone: "Telefon", isActive: "Durum" }} />;
}
