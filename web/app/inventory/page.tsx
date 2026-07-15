"use client";

import { Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { InventoryPage } from "../../components/InventoryPage";
import { EntityDetailPage } from "../../components/EntityDetailPage";
import { Spinner } from "../../components/Ui";

function InventoryContent() {
  const searchParams = useSearchParams();
  const id = searchParams.get("id");
  if (id) return <EntityDetailPage title="Parça detayı" eyebrow="Envanter" endpoint={`/v1/parts/${id}`} backHref="/inventory" labels={{ partNumber: "Parça numarası", name: "Parça adı", description: "Açıklama", unitOfMeasure: "Birim", isActive: "Durum" }} />;
  return <InventoryPage />;
}

export default function InventoryRoute() {
  return (
    <Suspense fallback={<div className="flex justify-center py-20 text-teal"><Spinner /></div>}>
      <InventoryContent />
    </Suspense>
  );
}
