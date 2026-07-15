/** @type {import('next').NextConfig} */
const nextConfig = {
  output: "export",
  basePath: "/BingehOS",
  assetPrefix: "/BingehOS/",
  trailingSlash: true,
  images: { unoptimized: true },
};

export default nextConfig;
