import { EntityListPage, EntityConfig } from "../../components/EntityListPage";

const config: EntityConfig = {
  title: "Varlıklar",
  eyebrow: "Varlık yönetimi",
  description: "Ekipman ve fiziksel varlık envanterini, kritik seviyeleriyle birlikte yönetin.",
  endpoint: "/v1/assets",
  basePath: "/assets",
  singular: "varlık",
  columns: [
    { key: "name", label: "Varlık" },
    { key: "serialNumber", label: "Seri no" },
    { key: "locationCode", label: "Konum" },
    { key: "criticality", label: "Kritiklik", type: "status" },
  ],
  createFields: [
    { name: "name", label: "Varlık adı", required: true, placeholder: "Kompresör 01" },
    { name: "serialNumber", label: "Seri numarası" },
    { name: "locationCode", label: "Konum kodu" },
    { name: "criticality", label: "Kritiklik", type: "select", required: true, options: ["Low", "Normal", "High", "Critical"] },
  ],
  editFields: [
    { name: "name", label: "Varlık adı", required: true },
    { name: "locationCode", label: "Konum kodu" },
    { name: "criticality", label: "Kritiklik", type: "select", required: true, options: ["Low", "Normal", "High", "Critical"] },
  ],
};

export default function AssetsPage() {
  return <EntityListPage config={config} />;
}
