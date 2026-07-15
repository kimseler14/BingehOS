import { EntityDetailPage } from "../../../../components/EntityDetailPage";

export default async function UserDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <EntityDetailPage title="Kullanıcı detayı" eyebrow="Yönetim · Identity" endpoint={`/v1/users/${id}`} backHref="/admin/users" labels={{ fullName: "Ad soyad", email: "E-posta", roles: "Roller", isActive: "Durum" }} />;
}
