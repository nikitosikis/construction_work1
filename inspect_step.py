import re, pathlib, json
p=next(pathlib.Path('.').glob('*.stp'))
s=p.read_text(encoding='cp1251')
e={int(k):v.replace('\n','') for k,v in re.findall(r'#(\d+)=(.*?);',s,re.S)}
def refs(v):return [int(x) for x in re.findall(r'#(\d+)',v)]
def vec(i):return [float(x) for x in re.search(r',\(([^()]+)\)',e[i]).group(1).split(',')]
def axis(i):
 r=refs(e[i]);return {'p':vec(r[0]),'z':vec(r[1]),'x':vec(r[2])}
for i,v in e.items():
 if v.startswith(('PLANE','CYLINDRICAL_SURFACE','CONICAL_SURFACE','TOROIDAL_SURFACE')):
  print(i,v.split('(')[0],axis(refs(v)[0]),v[v.rfind('#'):].split(',',1)[1])
pts=[vec(i) for i,v in e.items() if v.startswith('CARTESIAN_POINT')]
print('ALL POINT BOUNDS',[(min(p[a] for p in pts),max(p[a] for p in pts)) for a in range(3)])
