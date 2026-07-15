"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { DigitalTwinPage } from "../../components/DigitalTwinPage";
import { DigitalTwinDetailPage } from "../../components/DigitalTwinDetailPage";
import { Spinner } from "../../components/Ui";

function DigitalTwinContent() {
  const searchParams = useSearchParams();
  const id = searchParams.get("id");
  if (id) return <DigitalTwinDetailPage id={id} />;
  return <DigitalTwinPage />;
}

export default function DigitalTwinRoute() {
  return (
    <Suspense fallback={<div className="flex justify-center py-20 text-teal"><Spinner /></div>}>
      <DigitalTwinContent />
    </Suspense>
  );
}
