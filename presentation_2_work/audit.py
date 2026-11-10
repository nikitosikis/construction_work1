from pathlib import Path
from zipfile import ZipFile
import json, re, posixpath, sys, math
from lxml import etree as ET

work=Path(__file__).resolve().parent
file=Path(sys.argv[1]) if len(sys.argv)>1 else work/'candidate.pptx'
ns={'p':'http://schemas.openxmlformats.org/presentationml/2006/main',
    'a':'http://schemas.openxmlformats.org/drawingml/2006/main',
    'r':'http://schemas.openxmlformats.org/officeDocument/2006/relationships',
    'c':'http://schemas.openxmlformats.org/drawingml/2006/chart'}
notes=json.loads((work/'notes.json').read_text(encoding='utf8'))
results=json.loads((work/'results.json').read_text(encoding='utf8'))

def rels(z,part):
    p,n=posixpath.split(part)
    data=ET.fromstring(z.read(posixpath.join(p,'_rels',n+'.rels')))
    return {el.get('Id'):(el.get('Type'), el.get('Target').lstrip('/') if el.get('Target').startswith('/')
            else posixpath.normpath(posixpath.join(p,el.get('Target')))) for el in data}

with ZipFile(file) as z:
    assert not z.testzip(), 'Zip integrity'
    root=ET.fromstring(z.read('ppt/presentation.xml'))
    rel=rels(z,'ppt/presentation.xml')
    slides=[rel[r][1] for r in root.xpath('//p:sldId/@r:id',namespaces=ns)]
    assert len(slides)==13
    for i,part in enumerate(slides,1):
        xml=ET.fromstring(z.read(part))
        rls=rels(z,part)
        title=xml.xpath('//p:sp[p:nvSpPr/p:cNvPr[@name="title" or @name="Title 1" or @name="Title 4"]]//a:t/text()',namespaces=ns)
        n=next(target for kind,target in rls.values() if kind.endswith('/notesSlide'))
        note=ET.fromstring(z.read(n)).xpath('//a:t/text()',namespaces=ns)
        normalized=' '.join(' '.join(note).split())
        expected=notes[str(i)].split('\n\n')[1]
        assert ' '.join(expected.split()) in normalized,(i,'Missing or altered speaking notes')
        assert notes[str(i)].split('\n\n')[0] in normalized,(i,'Missing timing')
        native_tbl=xml.xpath('//a:tbl',namespaces=ns)
        assert bool(native_tbl)==(i in (4,8,9)),(i,'Unexpected tables')
        if i==9:
            numbers=[float(x.replace(',','.')) for x in native_tbl[0].xpath('a:tr[position()>1]/a:tc[2]//a:t/text()',namespaces=ns)]
            assert len(numbers)==4 and math.isclose(sum(numbers[:3]),numbers[-1],abs_tol=.01)
            assert math.isclose(numbers[-1],results['totalQ'],abs_tol=.005)
        native_chart=xml.xpath('//c:chart/@r:id',namespaces=ns)
        assert bool(native_chart)==(i==7),(i,'Unexpected charts')
        if i==7:
            chart=ET.fromstring(z.read(rls[native_chart[0]][1]))
            values=[float(x) for x in chart.xpath('//c:val//c:pt/c:v/text()',namespaces=ns)]
            assert all(math.isclose(a,b,abs_tol=.0006) for a,b in zip(values,results['sweep']['minGap'])) and len(values)==7
        pics=xml.xpath('//p:pic/p:blipFill/a:blip/@r:embed',namespaces=ns)
        if i in (3,6): assert any(rls[x][1] in z.namelist() for x in pics),(i,'Missing sourced image')
        if 2<=i<=12:
            bg=xml.xpath('//p:bg//a:blip/@r:embed',namespaces=ns)
            assert bg and all(z.read(rls[x][1])==(work/'itmo_body.jpg').read_bytes() for x in bg),(i,'Incorrect branded background')
            pagenum=xml.xpath('//p:sp[p:nvSpPr/p:cNvPr/@name="page-number"]//a:t/text()',namespaces=ns)
            assert pagenum==[f'{i:02}'],(i,'Wrong slide number')
        print(i, ' / '.join(title), 'note chars',len(normalized))
    print('Native Excel workbooks:',[f for f in z.namelist() if f.endswith('.xlsx')])
print('Spoken word count:',sum(len(re.findall(r'\b[\w]+\b',s.split('\n\n')[1])) for s in notes.values()))
