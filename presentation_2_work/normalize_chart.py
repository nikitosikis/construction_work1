from pathlib import Path
from zipfile import ZipFile
from lxml import etree as E
from decimal import Decimal
import json, re

work=Path(__file__).resolve().parent
data=json.loads((work/'results.json').read_text(encoding='utf8'))
ns={'c':'http://schemas.openxmlformats.org/drawingml/2006/chart',
    'a':'http://schemas.openxmlformats.org/drawingml/2006/main'}
with ZipFile(work/'candidate.pptx') as src, ZipFile(work/'portable_candidate.pptx','w') as out:
    count=0
    for member in src.infolist():
        raw=src.read(member)
        if re.fullmatch(r'ppt/charts/chart\d+\.xml',member.filename):
            chart=E.fromstring(raw)
            points=chart.xpath('//c:val/c:numLit/c:pt/c:v',namespaces=ns)
            assert len(points)==len(data['sweep']['minGap'])
            for el,x in zip(points,data['sweep']['minGap']):
                expected=Decimal(f'{x:.3f}')
                assert abs(Decimal(el.text)-expected)<Decimal('0.00000001')
                el.text=format(expected,'f').rstrip('0').rstrip('.')
            for e in chart.xpath('//c:ser/c:smooth',namespaces=ns): e.set('val','0')
            for marker in chart.xpath('//c:ser/c:marker/c:spPr',namespaces=ns):
                line=E.SubElement(marker,'{'+ns['a']+'}ln',w='9525')
                fill=E.SubElement(line,'{'+ns['a']+'}solidFill')
                E.SubElement(fill,'{'+ns['a']+'}srgbClr',val='9B00FF')
            raw=E.tostring(chart,encoding='UTF-8',xml_declaration=True,standalone=True)
            count+=1
        out.writestr(member,raw)
assert count==1
