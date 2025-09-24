import xml.etree.ElementTree as ET
from pathlib import Path
import re

def to_pascal_case(name: str) -> str:
    """Convert kebab-case or snake_case to PascalCase"""
    parts = re.split(r"[-_]", name)
    return "".join(p.capitalize() for p in parts if p)

def extract_paths(svg_file: Path):
    try:
        tree = ET.parse(svg_file)
        root = tree.getroot()

        ns = {"svg": "http://www.w3.org/2000/svg"}
        paths = root.findall(".//svg:path", ns)

        if not paths:
            return []

        base_name = to_pascal_case(svg_file.stem)
        results = []

        for i, path in enumerate(paths, start=1):
            d = path.get("d")
            if not d:
                continue

            if i == 1:
                key = f"Icons.{base_name}"
            else:
                key = f"Icons.{base_name}{i}"

            results.append(f'<StreamGeometry x:Key="{key}">{d}</StreamGeometry>')

        return results

    except Exception as e:
        print(f"[{svg_file}] Error: {e}")
        return []

def extract_paths_from_directory(directory):
    svg_files = Path(directory).glob("*.svg")
    all_results = []
    for svg_file in svg_files:
        all_results.extend(extract_paths(svg_file))
    return all_results

if __name__ == "__main__":
    import sys
    if len(sys.argv) != 2:
        print("Usage: python extract_paths.py <directory>")
    else:
        results = extract_paths_from_directory(sys.argv[1])
        for line in results:
            print(line)
