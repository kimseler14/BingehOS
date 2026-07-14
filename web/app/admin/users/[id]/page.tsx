import { EntityDetailPage } from "../../../../components/EntityDetailPage";

export default function UserDetailRoute({ params }: { params: { id: string } }) {
  return <EntityDetailPage title="Kullanıcı detayı" eyebrow="Yönetim · Identity" endpoint={`/v1/users/${params.id}`} backHref="/admin/users" labels={{ fullName: "Ad soyad", email: "E-posta", roles: "Roller", isActive: "Durum" }} />;
}
