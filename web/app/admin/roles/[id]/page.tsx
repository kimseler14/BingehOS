import { RoleDetailPage } from "../../../../components/RoleDetailPage";

export default function RoleDetailRoute({ params }: { params: { id: string } }) {
  return <RoleDetailPage id={params.id} />;
}
