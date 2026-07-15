"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { WorkOrdersPage } from "../../components/WorkOrdersPage";
import { EntityDetailPage } from "../../components/EntityDetailPage";
import { Spinner } from "../../components/Ui";

function WorkOrdersContent() {
  const searchParams = useSearchParams();
  const id = searchParams.get("id");
  if (id) return <EntityDetailPage title="İş emri detayı" eyebrow="Bakım operasyonu" endpoint={`/v1/work-orders/${id}`} backHref="/work-orders" labels={{ description: "Açıklama", assetId: "Varlık ID", status: "Durum" }} />;
  return <WorkOrdersPage />;
}

export default function WorkOrdersRoute() {
  return (
    <Suspense fallback={<div className="flex justify-center py-20 text-teal"><Spinner /></div>}>
      <WorkOrdersContent />
    </Suspense>
  );
}
