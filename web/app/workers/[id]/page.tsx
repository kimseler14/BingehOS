import { EntityDetailPage } from "../../../components/EntityDetailPage";

export default function WorkerDetailRoute({ params }: { params: { id: string } }) {
  return <EntityDetailPage title="Personel detayı" eyebrow="Ekip yönetimi" endpoint={`/v1/workers/${params.id}`} backHref="/workers" labels={{ firstName: "Ad", lastName: "Soyad", employeeNumber: "Sicil numarası", trade: "Uzmanlık", department: "Departman", phone: "Telefon", isActive: "Durum" }} />;
}
