"use client";

import { EntityListPage, EntityConfig } from "../../../components/EntityListPage";

const config: EntityConfig = {
  title: "Vergi Kayıtları",
  eyebrow: "Finans yönetimi",
  description: "KDV ve diğer vergi kayıtlarını görüntüleyin.",
  endpoint: "/v1/tax-records",
  basePath: "/finance/tax-records",
  singular: "vergi kaydı",
  canCreate: false,
  columns: [
    { key: "taxType", label: "Vergi türü" },
    { key: "taxRate", label: "Oran" },
    { key: "taxAmountMinor", label: "Tutar" },
    { key: "currency", label: "Para birimi" },
  ],
  createFields: [],
  detail: {
    title: "Vergi kaydı detayı",
    eyebrow: "Finans yönetimi",
    endpoint: (id: string) => `/v1/tax-records/${id}`,
    backHref: "/finance/tax-records",
    labels: { taxType: "Vergi türü", taxRate: "Oran", taxAmountMinor: "Tutar", currency: "Para birimi", invoiceId: "Fatura" },
  },
};

export default function TaxRecordsPage() {
  return <EntityListPage config={config} />;
}
