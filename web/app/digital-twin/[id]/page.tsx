import { DigitalTwinDetailPage } from "../../../components/DigitalTwinDetailPage";

export default function DigitalTwinDetailRoute({ params }: { params: { id: string } }) {
  return <DigitalTwinDetailPage id={params.id} />;
}
