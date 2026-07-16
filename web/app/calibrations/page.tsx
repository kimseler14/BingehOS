"use client";

import { EntityListPage, EntityConfig } from "../../components/EntityListPage";

const config: EntityConfig = {
  title: "Kalibrasyon Kayıtları",
  eyebrow: "Uygunluk yönetimi",
  description: "Cihaz ve ekipman kalibrasyon kayıtlarını yönetin.",
  endpoint: "/v1/calibrations",
  basePath: "/calibrations",
  singular: "kalibrasyon",
  columns: [
    { key: "calibrationDate", label: "Kalibrasyon", type: "date" },
    { key: "nextCalibrationDate", label: "Sonraki", type: "date" },
    { key: "result", label: "Sonuç", type: "status" },
    { key: "certifiedBy", label: "Onaylayan" },
  ],
  createFields: [
    { name: "assetId", label: "Varlık ID", required: true },
    { name: "calibrationDate", label: "Kalibrasyon tarihi", required: true },
    { name: "nextCalibrationDate", label: "Sonraki kalibrasyon", required: true },
    { name: "result", label: "Sonuç" },
    { name: "certifiedBy", label: "Onaylayan" },
  ],
  editFields: [
    { name: "nextCalibrationDate", label: "Sonraki kalibrasyon" },
    { name: "result", label: "Sonuç" },
    { name: "certifiedBy", label: "Onaylayan" },
    { name: "notes", label: "Notlar", type: "textarea" },
  ],
  detail: {
    title: "Kalibrasyon detayı",
    eyebrow: "Uygunluk yönetimi",
    endpoint: (id: string) => `/v1/calibrations/${id}`,
    backHref: "/calibrations",
    labels: { calibrationDate: "Kalibrasyon", nextCalibrationDate: "Sonraki", result: "Sonuç", certifiedBy: "Onaylayan", notes: "Notlar" },
  },
};

export default function CalibrationsPage() {
  return <EntityListPage config={config} />;
}
