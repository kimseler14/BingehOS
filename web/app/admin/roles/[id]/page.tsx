import { RoleDetailPage } from "../../../../components/RoleDetailPage";

export default async function RoleDetailRoute({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <RoleDetailPage id={id} />;
}
